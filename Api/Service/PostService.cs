using Api.Model;
using Api.Service;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

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

        public PostService( Context context, IConnectionService connectionService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
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
        /// sauf si les utlise est en mod eprive ne montre pas ses postes
        public async Task<List<Post>> GetAllPostsAsync()
        {
            var posts = await _context.Post.ToListAsync();
            // Pour chaque publication
            foreach (var post in posts)
            {
                if (!string.IsNullOrEmpty(post.MediaUrl))
                {
                    post.DownloadUrl = post.MediaUrl;
                }
            }

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
            Post post = await _context.Post.FindAsync(postId);

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
            post.CreateDate = DateTime.Now;
            post.UpdateDate = DateTime.Now;

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

            await _context.Post.AddAsync(post);
            await _context.SaveChangesAsync();

            Post postCreate = await _context.Post.FindAsync(post.Id);

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
            //var userInfo = _connectionService.GetCurrentUserInfo(_httpContextAccessor);
            //int userId = userInfo.Id;

            // if (userId == 0)
            //    throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");*/

            Post post = await _context.Post.FindAsync(postId);
            if (post != null)
            {
                _context.Post.Remove(post);
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
