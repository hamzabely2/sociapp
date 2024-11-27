using Api.Entity;
using Api.Model.DTO;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        /// <summary>
        ///get  user,
        /// </summary>
        /// <returns></returns>
        [HttpGet("get")]
        public async Task<ActionResult<List<User>>> GetUser()
        {
            try
            {
                var result = await _userService.GetUserAsync();
                string message = "utilisateur";
                return Ok(new { message, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        ///get all users,
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-users")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersAsync();
                string message = "list utilisateur";
                return Ok(new { message, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        ///login,
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login( LoginUserDTO user)
        {
            try
            {
                var token = await _userService.LoginAsync(user);
                string message = "le token a été cree avec succès";
                return Ok(new { message, token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// register
        /// </summary>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<List<Post>>> Register( User user)
        {
            try
            {
                var token = await _userService.RegisterAsync(user);
                string message = "le utilisateur a été cree avec succès";
                return Ok(new { message, token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

            /// <summary>
            /// update user
            /// </summary>
            /// <returns></returns>
            [HttpPut("update")]
            [Authorize()]

            public async Task<ActionResult<string>> UpdateUser(bool profilePrivacy)
            {
                try
                {
                    var token = await _userService.UpdateUserAsync(profilePrivacy);
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

