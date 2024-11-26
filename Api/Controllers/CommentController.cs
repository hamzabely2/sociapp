using Api.Entity;
using Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// craete comments
        /// </summary>
        /// <returns></returns>
        [HttpPost("create/{postId}")]
        //[Authorize()]
        public async Task<ActionResult<Comments>> CreateComment(Comments comment, int postId)
        {
            try
            {
                Comments result = await _commentService.CreateCommentsAsync(comment, postId);
                string message = "comments a été ajoutée avec succès";
                return Ok(new { message, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// get post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Comments>> DeleteComments(int commentId)
        {
            try
            {
                Comments result = await _commentService.DeleteCommentsAsync(commentId);
                string message = "comments a été supprime avec succès";
                return Ok(new { message, result });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

    }
}
