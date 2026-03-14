using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pharmacy.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration,
                            UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddDays(
                    double.Parse(_configuration["JWT:DurationInDays"]!)
                ),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}