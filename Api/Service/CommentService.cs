using Api.Entity;

namespace Api.Service
{
    public interface ICommentService
    {
        Task<Comments> CreateCommentsAsync(Comments comment,int postId);
        Task<Comments> DeleteCommentsAsync(int commentId);
    }

    public class CommentService : ICommentService
    {
        private readonly Context _context;
        private readonly IConnectionService _connectionService;

        public CommentService(Context context, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _context = context;
        }

        /// <summary>
        /// create commnets by users in the post 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Comments> CreateCommentsAsync(Comments comment,int postId)
        {

            var userInfo = _connectionService.GetCurrentUserInfo();
             if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            Post post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                throw new ArgumentException("le post ne existe pas");
            }

            comment.CreateDate = DateTime.Now;
            comment.UpdateDate = DateTime.Now;
            comment.PostId = postId;
            comment.UserId = userInfo.Id;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            Comments commentCreate = await _context.Comments.FindAsync(comment.Id);

            if (commentCreate != null)
            {
                return commentCreate;
            }
            else
            {
                throw new ArgumentException("L'action a échoué");
            }
        }


        /// <summary>
        /// delete comments by users in the post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Comments> DeleteCommentsAsync(int commentId)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();
             if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            Comments comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return comment;
            }
            else
            {
                throw new ArgumentException("L'action a échoué");
            }
        }
    }
}
