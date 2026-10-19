using PulseOps.Domain.Entities;

namespace PulseOps.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}