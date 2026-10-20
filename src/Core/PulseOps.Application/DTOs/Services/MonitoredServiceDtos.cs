namespace PulseOps.Application.DTOs.Services;

public record CreateServiceRequestDto(
    string Name,
    string Url,
    int CheckIntervalSeconds
);

public record MonitoredServiceDto(
    Guid Id,
    string Name,
    string Url,
    int CheckIntervalSeconds,
    bool IsActive,
    DateTime CreatedAt,
    ServiceHealthLogDto? LastCheck
);

public record ServiceHealthLogDto(
    int StatusCode,
    bool IsHealthy,
    long ResponseTimeMs,
    string? ErrorMessage,
    DateTime CheckedAt
);