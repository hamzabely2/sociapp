using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        /// get follow by user
        /// </summary>
        /// <returns></returns>
        /*[HttpGet("followed-users")]
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
        }*/

        /// <summary>
        /// fololow
        /// </summary>
        /// <returns></returns>
        [HttpPost("follow")]
        [Authorize()]
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
    }
}
