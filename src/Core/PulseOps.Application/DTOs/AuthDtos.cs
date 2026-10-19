namespace PulseOps.Application.DTOs;

public record RegisterRequestDto(string FirstName, string LastName, string Email, string Password, string OrganizationName);
public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime Expiration, string Email, string Role);
public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);