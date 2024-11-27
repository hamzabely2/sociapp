using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        /// <summary>
        /// get list follow by user
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-list-followed-users")]
        public async Task<IActionResult> GetFollowedUsers()
        {
            try
            {
                var followedUsers = await _followService.GetFollowedUsersAsync();
                return Ok(followedUsers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        /// <summary>
        /// fololow user
        /// </summary>
        /// <returns></returns>
        [HttpPost("followe-user")]
        public async Task<ActionResult<string>> FollowUser(int userIdFollowed)
        {
            try
            {
                var token = await _followService.FollowUserAsync(userIdFollowed);
                string message = "";
                return Ok(new { message, token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// unfololow user
        /// </summary>
        /// <returns></returns>
        [HttpPost("followe-user")]
        [Authorize()]
        public async Task<ActionResult<string>> UnFollowUser(int userIdFollowed)
        {
            try
            {
                var token = await _followService.UnFollowUserAsync(userIdFollowed);
                string message = "";
                return Ok(new { message, token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
