namespace PulseOps.Domain.Entities;

using PulseOps.Domain.Common;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Member"; // Admin, Member, Viewer

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    // Refresh Token Yapısı
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}