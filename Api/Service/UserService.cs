using Api.Entity;
using Api.Model.DTO;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;

namespace Api.Service
{
    public interface IUserService
    {
        Task<string> RegisterAsync(User user);
        Task<string> LoginAsync(LoginUserDTO user);
        Task<string> FollowUserAsync(int userIdFollowed);
        Task<string> UpdateUserAsync(bool profilePrivacy);
        Task<List<User>> GetAllUsersAsync();
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

        public UserService(Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _connectionService = connectionService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<string> RegisterAsync(User user)
        {

            var UserNmaeExiste = _context.Users.FindAsync(user.Id);
            if (UserNmaeExiste.Result != null)
                throw new ArgumentException("l'action a échoué : le username existe déjà");

            var passwordHash = _connectionService.HashPassword(user.Password);
            user.Password = passwordHash;
            user.ProfilePrivacy = false;
            user.UpdateDate  = DateTime.Now;
            user.CreateDate = DateTime.Now;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
            new Claim(ClaimTypes.Name, $"{user.UserName}"),
            new Claim(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            //create token
            var token = _connectionService.CreateToken(claims);
            _connectionService.AddTokenCookie(new JwtSecurityTokenHandler().WriteToken(token));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> LoginAsync(LoginUserDTO user)
        {
            User userExiste = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email).ConfigureAwait(false);
            if (userExiste == null || !_connectionService.VerifyPassword(user.Password, userExiste.Password))
                throw new ArgumentException("La connexion a échoué : e-mail ou mot de passe incorrect.");

            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userExiste.Id)),
                new Claim(ClaimTypes.Name, $"{userExiste.UserName}"),
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



        public async Task<string> FollowUserAsync(int userIdFollowed)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            User userExiste = await _context.Users.FindAsync(userIdFollowed).ConfigureAwait(false);
            if (userExiste == null)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas.");

            Follow newFollow = new Follow();
            newFollow.UpdateDate = DateTime.Now;
            newFollow.CreateDate = DateTime.Now;
            newFollow.UserId = userInfo.Id;
            newFollow.FollowUserId = userIdFollowed;

            await _context.Follow.AddAsync(newFollow);
            await _context.SaveChangesAsync();

            return "";

        }

        public async Task<string> UpdateUserAsync(bool profilePrivacy)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            User user = await _context.Users.FindAsync(userInfo.Id);

            user.ProfilePrivacy = profilePrivacy;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "la modification du profil a réussi";

            /// Update user
        }
    }
}
