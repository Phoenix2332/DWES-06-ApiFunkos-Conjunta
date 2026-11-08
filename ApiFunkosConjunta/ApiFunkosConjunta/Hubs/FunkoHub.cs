using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ApiFunkosConjunta.Hubs;

[Authorize]
public class FunkoHub : Hub {
    public override async Task OnConnectedAsync() {
        var usuarioId = Context.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(usuarioId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"usuario-{usuarioId}");

        var rol = Context.User?
            .FindFirst(ClaimTypes.Role)?.Value;

        if (rol == "Administrador") await Groups.AddToGroupAsync(Context.ConnectionId, "administradores");

        await base.OnConnectedAsync();
    }
}