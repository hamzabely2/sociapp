using Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Service
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync();
        Task<bool> DeleteNotificationAsync(int notificationId);  // Renvoie un booléen pour indiquer si la suppression a réussi
    }

    public class NotificationService : INotificationService
    {
        private readonly Context _context;
        private readonly IConnectionService _connectionService;

        public NotificationService(Context context, IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _context = context;
        }

        public async Task<List<Notification>> GetNotificationsAsync()
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userInfo.Id)
                                              .OrderByDescending(n => n.CreateDate)
                                              .ToListAsync();

            if (notifications == null || notifications.Count == 0)
            {
                // Vous pouvez renvoyer une réponse indiquant qu'il n'y a pas de notifications
                throw new ArgumentException("Aucune notification trouvée pour cet utilisateur.");
            }

            return notifications;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var userInfo = _connectionService.GetCurrentUserInfo();

            if (userInfo.Id == 0)
                throw new ArgumentException("L'action a échoué : l'utilisateur n'existe pas");

            var notification = await _context.Notifications
                                             .SingleOrDefaultAsync(n => n.Id == notificationId && n.UserId == userInfo.Id);

            if (notification == null)
            {
                throw new ArgumentException("Notification non trouvée.");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return true;  

        }
    }
}
