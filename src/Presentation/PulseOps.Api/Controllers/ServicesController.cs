using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseOps.Application.DTOs.Services;
using PulseOps.Domain.Entities;
using PulseOps.Infrastructure.Persistence;

namespace PulseOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Korumalı endpoint: Sadece JWT Token sahipleri erişebilir
public class ServicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequestDto dto)
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            return Unauthorized("Organizasyon bilgisi bulunamadı.");
        }

        var service = new MonitoredService
        {
            Name = dto.Name,
            Url = dto.Url,
            CheckIntervalSeconds = dto.CheckIntervalSeconds > 0 ? dto.CheckIntervalSeconds : 60,
            OrganizationId = organizationId
        };

        _context.MonitoredServices.Add(service);
        await _context.SaveChangesAsync();

        return Ok(new MonitoredServiceDto(
            service.Id,
            service.Name,
            service.Url,
            service.CheckIntervalSeconds,
            service.IsActive,
            service.CreatedAt,
            null
        ));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyServices()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            return Unauthorized("Organizasyon bilgisi bulunamadı.");
        }

        var services = await _context.MonitoredServices
            .Where(s => s.OrganizationId == organizationId)
            .Include(s => s.HealthLogs.OrderByDescending(l => l.CheckedAt).Take(1))
            .Select(s => new MonitoredServiceDto(
                s.Id,
                s.Name,
                s.Url,
                s.CheckIntervalSeconds,
                s.IsActive,
                s.CreatedAt,
                s.HealthLogs.Select(l => new ServiceHealthLogDto(
                    l.StatusCode,
                    l.IsHealthy,
                    l.ResponseTimeMs,
                    l.ErrorMessage,
                    l.CheckedAt
                )).FirstOrDefault()
            ))
            .ToListAsync();

        return Ok(services);
    }
}