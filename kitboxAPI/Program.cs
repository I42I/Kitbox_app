using Microsoft.EntityFrameworkCore;
using KitboxAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuration pour Docker/Production
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(50097); // Port fixe pour le container
});

// 🔹 Connexion DB - Utilise les variables d'environnement en production
var connectionString = Environment.GetEnvironmentVariable("KITBOX_DB_CONNECTION") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(10, 5)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    )
);

// 🔹 CORS pour l'application Avalonia
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAvalon", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔹 Sérialisation enum → string
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Test de connexion DB avec retry
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var maxRetries = 5;
    var retryCount = 0;
    
    while (retryCount < maxRetries)
    {
        try
        {
            await dbContext.Database.CanConnectAsync();
            Console.WriteLine("✅ Connexion à MariaDB réussie !");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            Console.WriteLine($"❌ Tentative {retryCount}/{maxRetries} - Erreur: {ex.Message}");
            if (retryCount < maxRetries)
            {
                await Task.Delay(5000); // Attendre 5 secondes
            }
        }
    }
}

// 🔹 Configuration middleware
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAvalon");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 API Kitbox démarrée sur le port 50097");
app.Run();
