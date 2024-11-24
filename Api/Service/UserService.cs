using Api.Entity;
using Api.Model.DTO;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Service
{
    public interface IUserService
    {
        Task<string> RegisterAsync(User user);
        Task<string> LoginAsync(LoginUserDTO user);
    }
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _accountKey;
        private readonly IConnectionService _connectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public UserService(Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) {
            _context = context;
            _connectionService = connectionService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(User user)
        {

            var UserNmaeExiste = _context.Users.FindAsync(user.Id);
            if (UserNmaeExiste.Result != null)
                throw new ArgumentException("l'action a échoué : le username existe déjà");

            var passwordHash = _connectionService.HashPassword(user.Password);
            user.Password = passwordHash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            //create token
            var token = _connectionService.CreateToken(claims);
            _connectionService.AddTokenCookie(new JwtSecurityTokenHandler().WriteToken(token), _httpContextAccessor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> LoginAsync(LoginUserDTO user)
        {
            User userExiste = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email).ConfigureAwait(false);
            if (userExiste == null  || !_connectionService.VerifyPassword(user.Password, userExiste.Password))
                throw new ArgumentException("La connexion a échoué : e-mail ou mot de passe incorrect.");

            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userExiste.Id)),
                new Claim(ClaimTypes.Name, $"{userExiste.FirstName} {userExiste.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = _connectionService.CreateToken(claims);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                HttpOnly = true,
                Secure = true,
                Domain = "sociapp.com"
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", new JwtSecurityTokenHandler().WriteToken(token), cookieOptions);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// Update user
    }
}
