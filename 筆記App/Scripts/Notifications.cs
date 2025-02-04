using System;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public static Notifications Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持單例實例
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notifications Channel",
            Importance = Importance.Default,
            Description = "Reminder notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);   

        // var notification = new AndroidNotification();
        // notification.Title = "hello!";
        // notification.Text = "Start your day with a reminder!";
        // notification.FireTime = System.DateTime.Now.AddSeconds(15);

        // var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");
    
        // if(AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        // {
        //     AndroidNotificationCenter.CancelAllDisplayedNotifications();
        //     AndroidNotificationCenter.SendNotification(notification, "channel_id");
        // }
    }

    void Update()
    {
        
    }

    // 發送通知
    public void ScheduleNotification(string title, string message, DateTime fireTime)
    {
        if (fireTime <= DateTime.Now)
        {
            Debug.LogError("Cannot schedule a notification in the past.");
            return;
        }

        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            FireTime = fireTime
        };

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
        Debug.Log($"Scheduled notification: {title} at {fireTime}");
    }

    // 取消所有通知
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        Debug.Log("All notifications canceled.");
    }

    // 檢查通知狀態
    public void CheckNotificationStatus(int notificationId)
    {
        var status = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId);
        Debug.Log($"Notification status: {status}");
    }

}
