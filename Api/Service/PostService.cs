using Api.Entity;
using Api.Service;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(Post post, IFormFile file);
        Task<Post> DeletePostAsync(int postId);
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
        /// get  all post
        /// </summary>
        /// <returns></returns>
        /// sauf si les utlise est en mode prive ne montre pas ses postes
        public async Task<List<Post>> GetAllPostsAsync()
        {
            var posts = await _context.Posts.ToListAsync();
            return posts;
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

        /// <summary>
        /// create post by users
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Post> CreatePostAsync(Post post, IFormFile file)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
            int userId = userInfo.Id;

            if (userId == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            post.CreateDate = DateTime.Now;
            post.UpdateDate = DateTime.Now;
            post.UserId = userId;

            if (!file.ContentType.StartsWith("image/") && !file.ContentType.StartsWith("video/"))
                 throw new ArgumentException("L'action a échoué: Type de fichier invalide.");

            //add file in storage
            if (file != null && file.Length > 0)
            {
                var blobServiceClient = new BlobServiceClient(_configuration["AzureStorage:ConnectionString"]);
                var containerClient = blobServiceClient.GetBlobContainerClient(_configuration["AzureStorage:ContainerName"]);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }

                post.MediaUrl = blobClient.Uri.ToString();
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            Post postCreate = await _context.Posts.FindAsync(post.Id);

            if (postCreate != null)
            {
                return postCreate;
            }
            else
            {
                throw new ArgumentException("L'action a échoué");
            }
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
