﻿using Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Service
{

    public interface IFollowService
    {
        Task<List<User>> GetFollowedUsersAsync();
        Task<string> FollowUserAsync(int userIdFollowed, int userId);
        Task<string> UnFollowUserAsync(int userIdFollowed);
    }
        public class FollowService : IFollowService
         {
        private readonly IConnectionService _connectionService;
        private readonly Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FollowService(Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _connectionService = connectionService;
            _httpContextAccessor = httpContextAccessor;
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


        public async Task<string> FollowUserAsync(int userIdFollowed, int userId)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            User userExiste = await _context.Users.FindAsync(userIdFollowed).ConfigureAwait(false);
            if (userExiste == null)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas.");

            bool alreadyFollowing = await _context.Follows
                .AnyAsync(f => f.UserId == userInfo.Id && f.FollowUserId == userIdFollowed)
                .ConfigureAwait(false);

            if (alreadyFollowing)
                throw new ArgumentException("L'action a échoué : vous suivez déjà cet utilisateur.");

            Follow newFollow = new Follow
            {
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now,
                UserId = userInfo.Id,
                FollowUserId = userIdFollowed
            };

            await _context.Follows.AddAsync(newFollow).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return "Maintenant, vous suivez cet utilisateur.";
        }

        public async Task<string> UnFollowUserAsync(int userIdFollowed)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");


            var followRelation = await _context.Follows
                .FirstOrDefaultAsync(f => f.UserId == userInfo.Id && f.FollowUserId == userIdFollowed)
                .ConfigureAwait(false);

            if (followRelation == null)
                throw new ArgumentException("L'action a échoué : vous ne suivez pas cet utilisateur.");

            _context.Follows.Remove(followRelation);

            // Sauvegarde les modifications
            await _context.SaveChangesAsync();

            return "vous ne suive plus le utlissateur";
        }
    }
}
