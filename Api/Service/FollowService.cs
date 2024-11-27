using Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Service
{

    public interface IFollowService
    {
        Task<List<User>> GetFollowedUsersAsync();
        Task<string> FollowUserAsync(int userIdFollowed);
        Task<string> UnFollowUserAsync(int userIdFollowed);
    }
        public class FollowService : IFollowService
         {
        private readonly IConnectionService _connectionService;
        private readonly Context _context;

        public FollowService(Context context, IConnectionService connectionService)
        {
            _context = context;
            _connectionService = connectionService;
        }


        /// <summary>
        /// Récupère tous les utilisateurs auxquels l'utilisateur connecté est abonné.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetFollowedUsersAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var userExists = await _context.Users.AnyAsync(u => u.Id == userInfo.Id);
            if (!userExists)
                throw new ArgumentException("L'utilisateur spécifié n'existe pas.");

            var followedUserIds = await _context.Follows
                .Where(f => f.UserId == userInfo.Id)
                .Select(f => f.FollowUserId) 
                .ToListAsync();

            var followedUsers = await _context.Users
                .Where(u => followedUserIds.Contains(u.Id))
                .ToListAsync();

            return followedUsers;
        }


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

            return "maintenant vous suive cet utilisateur";
        }
        /// <summary>
        /// unnfollow user
        /// </summary>
        /// <param name="userIdFollowed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> UnFollowUserAsync(int userIdFollowed)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur actuel n'existe pas");

            var userExists = await _context.Users.FindAsync(userIdFollowed).ConfigureAwait(false);
            if (userExists == null)
                throw new ArgumentException("L'action a échoué : l'utilisateur à ne plus suivre n'existe pas.");

            var followRelation = await _context.Follows
                .FirstOrDefaultAsync(f => f.Id == userInfo.Id && f.Id == userIdFollowed)
                .ConfigureAwait(false);

            if (followRelation == null)
                throw new ArgumentException("L'action a échoué : vous ne suivez pas cet utilisateur.");

            _context.Follows.Remove(followRelation);
            await _context.SaveChangesAsync();

            return "Vous ne suivez plus cet utilisateur.";
        }
    }
}
