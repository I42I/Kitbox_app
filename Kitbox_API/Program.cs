using System.Text;
using Kitbox_API.Configuration;
using Kitbox_API.Models;
using Kitbox_API.Repositories;
using Kitbox_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration DbContext
var connectionString = Environment.GetEnvironmentVariable("KITBOX_DB_CONNECTION") 
    ?? builder.Configuration.GetConnectionString("KitboxDb");

builder.Services.AddDbContext<KitboxContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Dépendances Repositories
builder.Services.AddScoped<ICabinetRepository, CabinetRepository>();
builder.Services.AddScoped<ICustomerOrderRepository, CustomerOrderRepository>();
builder.Services.AddScoped<ILockerRepository, LockerRepository>();
builder.Services.AddScoped<ILockerStockRepository, LockerStockRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierOrderRepository, SupplierOrderRepository>();

// Dépendances Services
builder.Services.AddScoped<ICabinetService, CabinetService>();
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<ILockerService, LockerService>();
builder.Services.AddScoped<ILockerStockService, LockerStockService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ISupplierOrderService, SupplierOrderService>();

// Configuration JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>();
var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? jwtSettings.Key;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// CORS pour Avalonia
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAvalonia", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAvalonia");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();