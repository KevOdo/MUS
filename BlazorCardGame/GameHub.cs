using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    private static Dictionary<string, string> Players = new(); // ConnectionId -> PlayerId

    public override async Task OnConnectedAsync()
    {
        string ip = Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        Console.WriteLine($"Connected: {Context.ConnectionId} from {ip}");
        await base.OnConnectedAsync();
    }

    public async Task RegisterPlayer(string playerId)
    {
        Players[Context.ConnectionId] = playerId;
        await Clients.Caller.SendAsync("PlayerRegistered", playerId);
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerJoined", Players[Context.ConnectionId]);
    }

    public async Task PlayCard(string gameId, string card)
    {
        var playerId = Players.GetValueOrDefault(Context.ConnectionId);
        if (playerId != null)
        {
            await Clients.Group(gameId).SendAsync("CardPlayed", playerId, card);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Players.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
