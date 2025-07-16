using MiniCMMS.Dtos;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    Task<IActionResult> Register(RegisterDto dto);
    Task<IActionResult> Login(LoginDto dto);
}