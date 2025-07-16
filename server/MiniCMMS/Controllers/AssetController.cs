using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MiniCMMS.Data;
using MiniCMMS.Models;
using MiniCMMS.Dtos;

namespace MiniCMMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets()
    {
        var assets = await _context.Assets.ToListAsync();
        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsset(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            return NotFound();
        return Ok(asset);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateAsset([FromBody] AssetDto dto)
    {
        var asset = new Asset
        {
            Name = dto.Name,
            MainLocation = dto.MainLocation,
            SubLocation = dto.SubLocation,
            Category = dto.Category,
            LastMaintained = DateOnly.FromDateTime(dto.LastMaintained)
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsset(int id, [FromBody] AssetDto dto)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            return NotFound();

        if (!string.IsNullOrEmpty(dto.Name))
            asset.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.MainLocation))
            asset.MainLocation = dto.MainLocation;

        if (!string.IsNullOrEmpty(dto.SubLocation))
            asset.SubLocation = dto.SubLocation;

        if (!string.IsNullOrEmpty(dto.Category))
                asset.Category = dto.Category;

        if (dto.LastMaintained != default(DateTime))
        asset.LastMaintained = DateOnly.FromDateTime(dto.LastMaintained.ToUniversalTime());
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            return NotFound();

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}