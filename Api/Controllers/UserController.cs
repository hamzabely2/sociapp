using Api.Entity;
using Api.Model.DTO;
using Api.Service;
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
        ///login,
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<List<Post>>> Login([FromForm] LoginUserDTO user)
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
        public async Task<ActionResult<List<Post>>> Register([FromForm] User user)
        {
            try
            {
                var token = await _userService.RegisterAsync(user);
                string message = "le user a été cree avec succès";
                return Ok(new { message, token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
          
        }
    }
}
