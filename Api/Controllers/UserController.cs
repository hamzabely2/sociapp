using Api.Entity;
using Api.Model.DTO;
using Api.Service;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public UserController(IUserService userService, ILogger<UserController> logger, TelemetryClient telemetryClient)
        {
            _userService = userService;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        ///get user,
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-user")]
        public async Task<ActionResult<List<User>>> GetUser()
        {
            try
            {
                var response = await _userService.GetUserAsync();
                string message = "utilisateur";
                return Ok(new { message, response });
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
                _telemetryClient.TrackEvent("ExampleEvent");
                _logger.LogInformation("{get-all-users} endpoint called.");

                var response = await _userService.GetAllUsersAsync();
                _logger.LogDebug("Response received: {Response}", response);
                string message = "list utilisateur";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data.");
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        ///login,
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDTO user)
        {
            try
            {
                var response = await _userService.LoginAsync(user);
                string message = "le token a été cree avec succès";
                return Ok(new { message, response });
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
        public async Task<ActionResult<List<Post>>> Register(User user)
        {
            try
            {
                var response = await _userService.RegisterAsync(user);
                string message = "le utilisateur a été cree avec succès";
                return Ok(new { message, response });
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
        [HttpPut("update-user/{userId}")]
        public async Task<ActionResult<string>> UpdateUser([FromBody] UpdateUserRequest request, int userId)
        {
            try
            {
                var response = await _userService.UpdateUserAsync(request.ProfilePrivacy, userId);
                string message = "La modification du profil a réussi";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class UpdateUserRequest
        {
            public bool ProfilePrivacy { get; set; }
        }
    }
  }

