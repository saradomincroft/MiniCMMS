using DotNetEnv;
using MiniCMMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Loads environment variables locally from .env
DotNetEnv.Env.Load();

// Reads database username and password from environment variables (from.env)
var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Get base connection string (host, port database) from config
var baseConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// Combines connection string with credentials
var fullConnection = $"{baseConnection};Username={dbUsername};Password={dbPassword}";

// Registers DbContext with PostgreSQL provider
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(fullConnection));

// Adds services to the container
builder.Services.AddControllers();

// Swagger for API docs in development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
