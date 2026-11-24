using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaStore.Application.DTOs;
using PizzaStore.Application.Interfaces;
using PizzaStore.Domain.Entities;
using PizzaStore.Infrastructure.Persistence;

namespace PizzaStore.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly PizzaDb _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

        public AuthService(PizzaDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto dto)
        {
            var normalizedUsername = dto.Username.Trim().ToLower();

            var exists = await _db.Users.AnyAsync(u => u.Username.ToLower() == normalizedUsername);
            if (exists) return null;

            var assignedRole = dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "Customer";

            var user = new ApplicationUser
            {
                Username = dto.Username.Trim(),
                Role = assignedRole
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return new LoginResponseDto(user.Username, user.Role, GenerateJwtToken(user));
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var normalizedUsername = dto.Username.Trim().ToLower();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername);

            if (user is null) return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, dto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new LoginResponseDto(user.Username, user.Role, GenerateJwtToken(user));
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = _config["Jwt:Key"] ?? "FallbackPizzaStoreSecretJWTKeyDoNotUseThisInProd!";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "PizzaStoreAPI";
            var jwtAudience = _config["Jwt:Audience"] ?? "PizzaStoreClient";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}