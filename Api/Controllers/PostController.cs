﻿using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize()]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;                                                                
        public PostController(IPostService postService, IConfiguration configuration)
        {
            _postService = postService;
        }


        /// <summary>
        /// get all post
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-user-posts")]
        public async Task<ActionResult<List<Post>>> GetAllPostsUser()
        {
            var posts = await _postService.GetAllPostsUserAsync();
            return Ok(posts);
        }

        /// <summary>
        /// get all post
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-follow-posts")]
        public async Task<ActionResult<List<Post>>> GetAllPosts()
        { 
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        /// <summary>
        /// get post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-post/{id}")]
        public async Task<ActionResult<Post>> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        /// <summary>
        /// create post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("create-post")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Post>> CreatePost( Post post)
        {
            try
            {
                var result = await _postService.CreatePostAsync(post);
                string message = "Le post a été ajouté avec succès";
                return Ok(new { message, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// delete post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-post/{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                Post result = await _postService.DeletePostAsync(postId);
                string message = "le post a été supprime avec succès";
                return Ok(new { message, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
