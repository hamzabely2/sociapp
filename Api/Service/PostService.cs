using Api.Entity;
using Api.Service;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsFromPublicProfilesAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(Post post);
        Task<Post> DeletePostAsync(int postId);
        Task<List<Post>> GetAllPostsUserAsync();
        Task<Post> UpdatePostAsync(int postId, Post post);
    }

    public class PostService : IPostService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _accountKey;
        private readonly IConnectionService _connectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public PostService(Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _connectionService = connectionService;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
            _accountKey = configuration["AzureStorage:AccountKey"];
            _configuration = configuration;
        }

        /// <summary>
        /// get post 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <summary>
        /// Get posts created by users with public profiles.
        /// </summary>
        public async Task<List<Post>> GetPostsFromPublicProfilesAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (_connectionService.GetCurrentUserInfo().Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var followedUserIds = await _context.Follows
                                                .Where(follow => follow.UserId == _connectionService.GetCurrentUserInfo().Id)
                                                .Select(follow => follow.FollowUserId)
                                                .ToListAsync();

            if (!followedUserIds.Any())
                return new List<Post>(); 

            var publicFollowedUserIds = await _context.Users
                                                      .Where(user => !user.ProfilePrivacy && followedUserIds.Contains(user.Id))
                                                      .Select(user => user.Id)
                                                      .ToListAsync();

            if (!publicFollowedUserIds.Any())
                return new List<Post>(); 

            var posts = await _context.Posts
                                       .Where(post => publicFollowedUserIds.Contains(post.UserId))
                                       .OrderByDescending(post => post.CreateDate)
                                       .ToListAsync();

            return posts;
        }

        /// <summary>
        /// get post create by user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<Post>> GetAllPostsUserAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var userPosts = await _context.Posts
                                            .Where(post => post.UserId == userInfo.Id)
                                            .OrderByDescending(post => post.CreateDate)
                                            .ToListAsync();

            return userPosts;
        }


        /// <summary>
        /// get post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///  sauf si les utlise est en mod eprive ne montre pas ses postes
        public async Task<Post> GetPostByIdAsync(int postId)
        {
            Post post = await _context.Posts.FindAsync(postId);

            if (post != null)
            {
                return post;
            }
            else
            {
                throw new ArgumentException("L'action a échoué");
            }
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (_connectionService.GetCurrentUserInfo().Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var file = post.MediaUrl;

            post.CreateDate = DateTime.Now;
            post.UpdateDate = DateTime.Now;
            post.UserId = userInfo.Id;

            if (_configuration["AzureStorage:ConnectionString"] != "")
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("L'action a échoué : Aucun fichier n'a été téléchargé.");

                if (!file.ContentType.StartsWith("image/") && !file.ContentType.StartsWith("video/"))
                    throw new ArgumentException("L'action a échoué : Type de fichier invalide.");

                var blobServiceClient = new BlobServiceClient(_configuration["AzureStorage:ConnectionString"]);
                string containerName = "";
                if (file.ContentType.StartsWith("image/"))
                {
                    containerName = "image";
                }
                else if (file.ContentType.StartsWith("video/"))
                {
                    containerName = "video";
                }

                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }

                post.DownloadUrl = blobClient.Uri.ToString();
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var followers = await _context.Follows
                .Where(f => f.FollowUserId == userInfo.Id)
                .ToListAsync();

            var notifications = followers.Select(follower => new Notification
            {
                Message = $"{userInfo.UserName} a : publié un nouveau post.",
                UserId = follower.UserId,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }).ToList();

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            return post;
        }

        /// <summary>
        /// update post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Post> UpdatePostAsync(int postId, Post post)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            // Récupérer le post existant
            var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (existingPost == null)
                throw new ArgumentException("Le post spécifié n'existe pas");

            if (existingPost.UserId != userInfo.Id)
                throw new UnauthorizedAccessException("Vous ne pouvez modifier que vos propres posts");

            var file = post.MediaUrl;

            // Mettre à jour les  du post
            existingPost.Title = post.Title ?? existingPost.Title;
            existingPost.Type = post.Type ?? existingPost.Type;
            existingPost.UpdateDate = DateTime.Now;

            if (file != null && file.Length > 0)
            {
                if (!file.ContentType.StartsWith("image/") && !file.ContentType.StartsWith("video/"))
                    throw new ArgumentException("L'action a échoué : Type de fichier invalide.");

                var blobServiceClient = new BlobServiceClient(_configuration["AzureStorage:ConnectionString"]);
                string containerName = "";

                if (file.ContentType.StartsWith("image/"))
                {
                    containerName = "image";
                }
                else if (file.ContentType.StartsWith("video/"))
                {
                    containerName = "video";
                }

                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Générer un nouveau nom pour le fichier
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }

                existingPost.DownloadUrl = blobClient.Uri.ToString();
            }

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();

            var followers = await _context.Follows
                .Where(f => f.FollowUserId == userInfo.Id)
                .ToListAsync();

            var notifications = followers.Select(follower => new Notification
            {
                Message = $"{userInfo.UserName} a mis à jour un post.",
                UserId = follower.UserId,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }).ToList();

            // Ici vous pourriez envoyer les notifications (facultatif selon votre implémentation)

            return existingPost;
        }





        /// <summary>
        /// delete post by users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Post> DeletePostAsync(int postId)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            int userId = userInfo.Id;

             if (userId == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            Post post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return post;
            }
            else
            {
                throw new ArgumentException("L'action a échoué");
            }
        }
    }
}
