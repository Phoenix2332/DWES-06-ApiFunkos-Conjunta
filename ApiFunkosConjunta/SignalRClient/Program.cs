
using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("=== Cliente de pruebas SignalR ===");
Console.Write("Introduce el JWT: ");

var token = Console.ReadLine();

if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("Debes introducir un token.");
    return;
}

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5087/hubs/funkos", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult<string?>(token);
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<object>("FunkoCreado", funko =>
{
    Console.WriteLine("\n[EVENTO] FunkoCreado");
    Console.WriteLine(funko);
});

connection.On<object>("FunkoActualizado", funko =>
{
    Console.WriteLine("\n[EVENTO] FunkoActualizado");
    Console.WriteLine(funko);
});

connection.On<object>("FunkoEliminado", funko =>
{
    Console.WriteLine("\n[EVENTO] FunkoEliminado");
    Console.WriteLine(funko);
});

try
{
    await connection.StartAsync();

    Console.WriteLine("\nConectado correctamente a SignalR.");
    Console.WriteLine("Esperando eventos...");
    Console.WriteLine("Pulsa ENTER para desconectar.");

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"\nError de conexión: {ex.Message}");
}
finally
{
    await connection.DisposeAsync();
}
