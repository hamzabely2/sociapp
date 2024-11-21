using Api.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Service
{
    public interface IConnectionService
    {
        UserInfo GetCurrentUserInfo(IHttpContextAccessor _httpContextAccessor);
    }

    public class ConnectionService : IConnectionService
    {
        private readonly IConfiguration _configuration;

        public ConnectionService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public UserInfo GetCurrentUserInfo(IHttpContextAccessor _httpContextAccessor)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

            return new UserInfo()
            {
                Id = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value),
                UserName = principal.FindFirst(ClaimTypes.Name)?.Value,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
            };
        }
    }
}
