using MiniCMMS.Dtos;
using Microsoft.AspNetCore.Mvc;
using MiniCMMS.Controllers;

public interface IAuthService
{
    Task<IActionResult> Register(RegisterDto dto);
    Task<IActionResult> Login(LoginDto dto);
}