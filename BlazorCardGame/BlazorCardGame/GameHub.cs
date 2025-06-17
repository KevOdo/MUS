using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class GameHub : Hub
{
    private static Dictionary<string, (string PlayerId, string PlayerName)> Players = new(); // ConnectionId -> (PlayerId, PlayerName)
    private static ConcurrentDictionary<string, HashSet<string>> GamePlayers = new(); // gameId -> set of PlayerIds
    private static ConcurrentDictionary<string, Dictionary<string, string>> GamePlayerNames = new(); // gameId -> (PlayerId -> PlayerName)

    public override async Task OnConnectedAsync()
    {
        string ip = Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        Console.WriteLine($"Connected: {Context.ConnectionId} from {ip}");
        await base.OnConnectedAsync();
    }

    public async Task RegisterPlayer(string playerId, string playerName)
    {
        Players[Context.ConnectionId] = (playerId, playerName);
        await Clients.Caller.SendAsync("PlayerRegistered", playerId);
    }

    public async Task JoinGame(string gameId)
    {
        if (!Players.TryGetValue(Context.ConnectionId, out var playerInfo))
        {
            await Clients.Caller.SendAsync("JoinFailed", "Player not registered.");
            return;
        }
        var playerId = playerInfo.PlayerId;
        var playerName = playerInfo.PlayerName;
        var players = GamePlayers.GetOrAdd(gameId, _ => new HashSet<string>());
        var playerNames = GamePlayerNames.GetOrAdd(gameId, _ => new Dictionary<string, string>());
        lock (players)
        {
            if (players.Count >= 4)
            {
                Clients.Caller.SendAsync("JoinFailed", "Game is full (4 players max).");
                return;
            }
            players.Add(playerId);
            playerNames[playerId] = playerName;
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerJoined", playerName);
        await Clients.Group(gameId).SendAsync("PlayerList", playerNames.Values.ToArray());
    }

    public async Task PlayCard(string gameId, string card)
    {
        if (!Players.TryGetValue(Context.ConnectionId, out var playerInfo))
            return;
        var playerName = playerInfo.PlayerName;
        await Clients.Group(gameId).SendAsync("CardPlayed", playerName, card);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (Players.TryGetValue(Context.ConnectionId, out var playerInfo))
        {
            foreach (var kvp in GamePlayers)
            {
                lock (kvp.Value)
                {
                    if (kvp.Value.Remove(playerInfo.PlayerId))
                    {
                        if (GamePlayerNames.TryGetValue(kvp.Key, out var nameDict))
                        {
                            nameDict.Remove(playerInfo.PlayerId);
                            Clients.Group(kvp.Key).SendAsync("PlayerList", nameDict.Values.ToArray());
                        }
                    }
                }
            }
        }
        Players.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
