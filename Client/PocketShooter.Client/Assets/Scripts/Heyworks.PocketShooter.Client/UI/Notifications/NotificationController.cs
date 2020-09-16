using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Notifications
{
    public class NotificationController : MonoBehaviour
    {
       private readonly IList<Notification> notifications;

       public NotificationController(IList<Notification> notifications)
       {
           this.notifications = notifications;
       }

       public void Notify()
       {
           foreach (var notification in notifications)
           {
               notification.Notify();
           }
       }
    }
}
