using Api.Entity;

namespace Api.Service
{

    public interface IFollowService
    {
       // Task<List<User>> GetFollowedUsersAsync();
        Task<string> FollowUserAsync(int userIdFollowed);
    }
        public class FollowService : IFollowService
    {

        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _accountKey;
        private readonly IConnectionService _connectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public FollowService(Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _connectionService = connectionService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


            /// <summary>
            /// Récupère tous les utilisateurs auxquels l'utilisateur connecté est abonné.
            /// </summary>
            /// <returns>Liste des utilisateurs abonnés.</returns>
        /*    public async Task<List<User>> GetFollowedUsersAsync()
        {
            // Récupérer l'utilisateur actuellement connecté
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            // Vérifie si l'utilisateur existe dans la base
            var userExists = await _context.Users.AnyAsync(u => u.Id == userInfo.Id);
            if (!userExists)
                throw new ArgumentException("L'utilisateur spécifié n'existe pas.");

            // Récupérer les IDs des utilisateurs suivis
            var followedUserIds = await _context.Follows
                .Where(f => f.UserId == userInfo.Id)
                .Select(f => f.Id)
                .ToListAsync();

            // Récupérer les informations des utilisateurs suivis
            var followedUsers = await _context.Users
                .Where(u => followedUserIds.Contains(u.Id))
                .ToListAsync();

            return followedUsers;
        }*/

        public async Task<string> FollowUserAsync(int userIdFollowed)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            User userExiste = await _context.Users.FindAsync(userIdFollowed).ConfigureAwait(false);
            if (userExiste == null)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'ex iste pas.");

            Follow newFollow = new Follow();
            newFollow.UpdateDate = DateTime.Now;
            newFollow.CreateDate = DateTime.Now;
            newFollow.UserId = userInfo.Id;
            newFollow.FollowUserId = userIdFollowed;

            await _context.Follows.AddAsync(newFollow);
            await _context.SaveChangesAsync();

            return "";

        }
    }
}
