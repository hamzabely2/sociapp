using Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

        public CommentService(Context context)
        {

            _context = context;
        }

        /// <summary>
        /// create commnets by users in the post 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Comments> CreateCommentsAsync(Comments comment,int postId)
        {

            //var userInfo = _connectionService.GetCurrentUserInfo(_httpContextAccessor);
            //int userId = userInfo.Id;

            // if (userId == 0)
            //    throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");*/

            Post post = await _context.Post.FindAsync(postId);

            if (post == null)
            {
                throw new ArgumentException("le post ne existe pas");
            }

            comment.CreateDate = DateTime.Now;
            comment.UpdateDate = DateTime.Now;
            comment.PostId = postId;
            //comment.UserId = userId;
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
            //var userInfo = _connectionService.GetCurrentUserInfo(_httpContextAccessor);
            //int userId = userInfo.Id;

            // if (userId == 0)
            //    throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");*/

            var comment = await _context.Comments.FindAsync(commentId);
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
