﻿using Api.Entity;
using Api.Service;
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
        public async Task<ActionResult<List<User>>> GetFollowedUsers()
        {
            try
            {
                var response = await _followService.GetFollowedUsersAsync();
                string message = "Liste des utilisateurs suivis";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        
        }

        /// <summary>
        /// fololow user
        /// </summary>
        /// <returns></returns>
        [HttpPost("followe-user/{userIdFollowed}")]
        public async Task<ActionResult<string>> FollowUser(int userIdFollowed,int userId)
        {
            try
            {
                var response = await _followService.FollowUserAsync(userIdFollowed, userId);
                string message = "maintenant vous suive cet utilisateur";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// unfollow user
        /// </summary>
        /// <returns></returns>
        [HttpDelete("unfollowe-user/{userIdFollowed}")]
        public async Task<ActionResult<string>> UnFollowUser(int userIdFollowed)
        {
            try
            {
                var response = await _followService.UnFollowUserAsync(userIdFollowed);
                string message = "L'utilisateur n'est plus suivi";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
