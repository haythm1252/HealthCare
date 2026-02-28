using HealthCare.Application.Common.Settings;
using HealthCare.Application.Services;
using HealthCare.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthCare.Infrastructure.Services;

public class JwtService(IOptions<JwtSettings> jwtSerttings) : IJwtService
{
    private readonly JwtSettings _jwtSerttings = jwtSerttings.Value;

    public (string token, int expireIn) GenerateToken(ApplicationUser user, string role)
    {
        var claims = new Claim[]
        {
                new(JwtRegisteredClaimNames.Sub,user.Id),
                new(JwtRegisteredClaimNames.Email,user.Email!),
                new(JwtRegisteredClaimNames.Name,user.Name),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, role)
        };

        var symmertriceKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSerttings.Key));

        var singCraditionals = new SigningCredentials(symmertriceKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _jwtSerttings.Issuer,
            audience: _jwtSerttings.Audience,
            signingCredentials: singCraditionals,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSerttings.ExpiryMinutes)
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(jwt), expireIn: _jwtSerttings.ExpiryMinutes * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmertriceKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSerttings.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmertriceKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }

    }
}
