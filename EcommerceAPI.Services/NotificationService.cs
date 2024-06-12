using FirebaseNotification = FirebaseAdmin.Messaging.Notification;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace EcommerceAPI.Services
{
    public interface INotificationService : IService<Notification>
    {
        Task AddNotification(Notification notification, DateTime? ScheduledDate = null);
    }

    public class NotificationService : Service<Notification>, INotificationService
    {
        public NotificationService(ITrackableRepository<Notification> repo) : base(repo)
        {

        }

        public async Task AddNotification(Notification notification, DateTime? ScheduledDate = null)
        {
            if (FirebaseApp.DefaultInstance == null)
                FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebasekey.json")) });

            var messaging = FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance;
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new FirebaseNotification
                {
                    Title = notification.Title,
                    Body = notification.Text,
                    ImageUrl = notification.Image,
                },
                Data = new Dictionary<string, string>()
                {
                    ["title"] = notification.Title,
                    ["text"] = notification.Text,
                    ["image"] = notification.Image,
                    ["url"] = notification.Url,
                },
                Topic = notification.Topic,
                Apns = new FirebaseAdmin.Messaging.ApnsConfig
                {
                    Aps = new FirebaseAdmin.Messaging.Aps { MutableContent = true },
                    FcmOptions = new FirebaseAdmin.Messaging.ApnsFcmOptions { ImageUrl = notification.Image }
                },
            };
            Insert(notification);

            if (ScheduledDate is null)
                await messaging.SendAsync(message);
        }
    }


    //public class OneSignalNotification
    //{
    //    public string app_id { get; set; } = "00f7edbe-f7d4-4e8b-a5b0-5c0ebb505e20";
    //    public Dictionary<string, string> contents { get; set; }
    //    public bool content_available { get; set; } = true;
    //}
    //public class TopicNotification : OneSignalNotification
    //{
    //    public TopicNotification(OneSignalNotification n)
    //    {
    //        this.contents = n.contents;
    //    }
    //    public string[] included_segments { get; set; }
    //}
    //public class OneSignalUserNotification : OneSignalNotification
    //{
    //    public OneSignalUserNotification(OneSignalNotification n)
    //    {
    //        this.contents = n.contents;
    //    }
    //    public string[] include_player_ids { get; set; }
    //    public string channel_for_external_user_ids { get; init; } = "push";
    //}
}
