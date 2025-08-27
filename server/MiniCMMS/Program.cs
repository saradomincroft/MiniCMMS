using DotNetEnv;
using MiniCMMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Build app
var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------

var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var baseConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var fullConnection = $"{baseConnection};Username={dbUsername};Password={dbPassword}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(fullConnection)
);

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- CORS ----------
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------- JWT Authentication ----------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                ?? throw new InvalidOperationException("JWT_SECRET_KEY not set");

var key = Encoding.UTF8.GetBytes(secretKey);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- Build App ----------
var app = builder.Build();

// ---------- Middleware Pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// CORS before Authentication/Authorization (remember!!!)
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run app
app.Run();
