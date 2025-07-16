using Xunit;
using Moq;
using MiniCMMS.Controllers;
using MiniCMMS.Data;
using MiniCMMS.Models;
using MiniCMMS.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.Json;

public class AuthControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthController _controller;
    private readonly Mock<DbSet<User>> _mockUserDbSet;
    private readonly List<User> _users;

    public AuthControllerTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["JwtSettings:Key"]).Returns("SuperTestKey");
        _mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");

        _users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "testtestington",
                Email = "test@test.com",
                Role = "Technician",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Password123!")
            }
        };

        _mockUserDbSet = new Mock<DbSet<User>>();
        var queryable = _users.AsQueryable();

        _mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
        _mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
        _mockUserDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        _mockUserDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());


        _mockUserDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .Returns<object[]>(ids =>
            {
                var id = (int)ids[0];
                var user = _users.FirstOrDefault(u => u.Id == id);
                return new ValueTask<User>(user);
            });

        _mockUserDbSet.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(user =>
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
        });

        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockContext.Setup(c => c.Users).Returns(_mockUserDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "SuperTestKey12345678901234567890");

        _controller = new AuthController(_mockContext.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUserIsValid()
    {
        var dto = new RegisterDto
        {
            FirstName = "Sara",
            LastName = "Croft",
            Email = "saracroft@test.com",
            Password = "Password123!",
            Role = "Manager"
        };

        var result = await _controller.Register(dto);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultDict = Assert.IsType<Dictionary<string, string>>(okResult.Value);
        Assert.Equal("User registered successfully.", resultDict["message"]);
        Assert.Contains(_users, u => u.Email == dto.Email);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailExists()
    {
        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "Testington",
            Email = "test@test.com",
            Password = "Password123!",
            Role = "Technician"
        };

        var result = await _controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsOk_WithValidCredentials_UsingUsername()
    {
        var dto = new LoginDto("testtestington", "Password123!");

        var result = _controller.Login(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        string json = JsonSerializer.Serialize(okResult.Value);
        var loginResult = JsonSerializer.Deserialize<LoginResponse>(json);

        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.token);
        Assert.Equal("testtestington", loginResult.username);
        Assert.Equal("Technician", loginResult.role);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WithInvalidPassword()
    {
        var dto = new LoginDto("testtestington", "WrongPassword");

        var result = _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        var dto = new LoginDto("wooghostuser", "PasswordGhostyGhost!");

        var result = _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    public class LoginResponse
    {
        public string token { get; set; } = "";
        public string username { get; set; } = "";
        public string role { get; set; } = "";
    }

}