using System;

namespace BlazorCardGame.Shared
{
    public class PlayerService
    {
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();
        public string? PlayerName { get; set; }
    }
}
