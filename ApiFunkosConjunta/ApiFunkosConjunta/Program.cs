using ApiFunkosConjunta.Common.Json;
using ApiFunkosConjunta.Data.Seed;
using ApiFunkosConjunta.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter()));
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

await CategoriaSeeder.SeedAsync(app.Services);

await ProveedorSeeder.SeedAsync(app.Services);

await FunkoSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();