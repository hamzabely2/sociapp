using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]

    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService) 
        {
            _notificationService = notificationService;
        }

            /// <summary>
            /// Récupérer toutes les notifications d'un utilisateur
            /// </summary>
            /// <param name="userId"></param>
        [HttpGet("get-notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var response = await _notificationService.GetNotificationsAsync();
                string message = "List notifications";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprimer une notification par son ID
        /// </summary>
        /// <param name="notificationId"></param>
        [HttpDelete("delete-notification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var response = await _notificationService.DeleteNotificationAsync(notificationId);
                string message = "La notification a été supprimée avec succès.";
                return Ok(new { message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
