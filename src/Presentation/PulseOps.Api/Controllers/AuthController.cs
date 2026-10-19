using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseOps.Application.Common.Interfaces;
using PulseOps.Application.DTOs;
using PulseOps.Domain.Entities;
using PulseOps.Infrastructure.Persistence;

namespace PulseOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(ApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            return BadRequest(new { Message = "Bu e-posta adresi zaten kullanımda." });

        var organization = new Organization
        {
            Name = request.OrganizationName ?? "Default Organization",
            Slug = (request.OrganizationName ?? "default-org")
                    .ToLower()
                    .Replace(" ", "-")
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        // BCrypt ile güvenli şifre hash'leme
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            OrganizationId = organization.Id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Ok(new
        {
            Message = "Kullanıcı başarıyla kaydedildi.",
            Token = token,
            UserId = user.Id,
            OrganizationId = organization.Id
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // BCrypt ile hash doğrulaması
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "E-posta veya şifre hatalı." });
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Ok(new
        {
            Message = "Giriş başarılı.",
            Token = token,
            User = new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                OrganizationName = user.Organization?.Name
            }
        });
    }
}