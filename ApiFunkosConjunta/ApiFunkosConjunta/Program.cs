using ApiFunkosConjunta.Common.Json;
using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Data.Seed;
using ApiFunkosConjunta.Hubs;
using ApiFunkosConjunta.Infrastructure;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter()));

builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// Reiniciar las bases de datos únicamente en Development.
if (app.Environment.IsDevelopment()) {
    await using var scope = app.Services.CreateAsyncScope();

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

    await mongoDatabase.Client.DropDatabaseAsync(
        mongoDatabase.DatabaseNamespace.DatabaseName);

    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
                                ?? throw new InvalidOperationException("No se ha configurado la conexión de Redis.");

    var redisConfiguration = ConfigurationOptions.Parse(redisConnectionString);
    redisConfiguration.AllowAdmin = true;

    await using var redis = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);

    var redisServer = redis.GetServer(redis.GetEndPoints().First());

    await redisServer.FlushDatabaseAsync();
}

await CategoriaSeeder.SeedAsync(app.Services);

await UsuarioSeeder.SeedAsync(app.Services);

await ProveedorSeeder.SeedAsync(app.Services);

await FunkoSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

if (app.Environment.IsDevelopment()) 
    app.UseHttpsRedirection();

app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<FunkoHub>("/hubs/funkos");

app.MapGraphQL()
    .WithOptions(options =>
        options.Tool.Enable = app.Environment.IsDevelopment());

app.Run();