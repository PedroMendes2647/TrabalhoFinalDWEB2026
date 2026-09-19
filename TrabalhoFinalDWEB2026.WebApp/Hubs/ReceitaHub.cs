using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TrabalhoFinalDWEB2026.WebApp.Hubs {
    [Authorize]
    public class ReceitaHub : Hub {
        // Hub bidirecional do SignalR
        // Permite broadcast geral ou direcionamento por UserId/Role
    }
}
