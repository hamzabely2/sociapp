using Api.Entity;
using Api.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Service
{
    public interface IUserService
    {
        Task<string> RegisterAsync(User user);
        Task<string> LoginAsync(LoginUserDTO user);
        Task<string> UpdateUserAsync(bool profilePrivacy);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserAsync();
    }
    public class UserService : IUserService
    {
        private readonly IConnectionService _connectionService;
        private readonly Context _context;

        public UserService(Context context, IConnectionService connectionService)
        {
            _context = context;
            _connectionService = connectionService;
        }

        /// <summary>
        /// get user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<User> GetUserAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            return await _context.Users.FindAsync(userInfo.Id);
        }

        /// <summary>
        /// get all users
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// register
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> RegisterAsync(User user)
        {
            User userExiste = await _context.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName).ConfigureAwait(false);
            if (userExiste != null)
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

            return new JwtSecurityTokenHandler().WriteToken(_connectionService.CreateToken(claims));
        }

        /// <summary>
        /// login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

            return new JwtSecurityTokenHandler().WriteToken(_connectionService.CreateToken(claims));
        }

        /// <summary>
        /// update user
        /// </summary>
        /// <param name="profilePrivacy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
        }
    }
}
