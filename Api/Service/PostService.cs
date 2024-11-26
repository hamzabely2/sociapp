﻿using Api.Service;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(Post post);
        Task<Post> DeletePostAsync(int postId);
        Task<List<Post>> GetAllPostsUserAsync();
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
        public async Task<List<Post>> GetAllPostsAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var followedUserIds = await _context.Follows
                .Where(f => f.UserId == userInfo.Id) 
                .Select(f => f.FollowUserId)
                .ToListAsync();

            var posts = await _context.Posts
                .Where(p => followedUserIds.Contains(p.UserId)
                            && !_context.Users.Any(u => u.Id == p.UserId && u.ProfilePrivacy))
                .ToListAsync();

            return posts;
        }

        /// <summary>
        /// get post create by user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// sauf si les utlise est en mod eprive ne montre pas ses postes
        public async Task<List<Post>> GetAllPostsUserAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var userPosts = await _context.Posts
                                            .Where(post => post.UserId == 1)
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
            

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var file = post.MediaUrl;

            post.CreateDate = DateTime.Now;
            post.UpdateDate = DateTime.Now;
            post.UserId = userInfo.Id;

            if (file == null || file.Length == 0)
                throw new ArgumentException("L'action a échoué : Aucun fichier n'a été téléchargé.");

            if (!file.ContentType.StartsWith("image/") && !file.ContentType.StartsWith("video/"))
                throw new ArgumentException("L'action a échoué : Type de fichier invalide.");

            // Ajout du fichier dans le stockage
            var blobServiceClient = new BlobServiceClient(_configuration["AzureStorage:ConnectionString"]);
            string containerName = "";
            if (file.ContentType.StartsWith("image/"))
            {
                containerName = "image";
            }else if (file.ContentType.StartsWith("video/"))
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

            // Stockez l'URL du fichier dans la propriété DownloadUrl
            post.DownloadUrl = blobClient.Uri.ToString();


            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
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
