using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Java.Lang;
using Java.Net;
using Java.Util.Logging;

namespace CrossAudioManager.Platforms.Droid
{
    public class AndroidNotificationManager : INotificationManager
    {
        public NotificationManager notificationManager;
        private Notification.Builder notificationBuilder;
        private Notification notification;
        private string channelId = "ramsey-network-notification-channel";

        #region Media Control Actions

        private Notification.Action previousAction;
        private Notification.Action nextAction;
        private Notification.Action skipBackwardAction;
        private Notification.Action skipForwardAction;
        private Notification.Action playAction;
        private Notification.Action pauseAction;
        private Notification.Action destroyAction;

        #endregion

        public AndroidNotificationManager()
        {
            notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
        }

        public void BuildNotification(IMediaItem mediaItem, double position = -1, double duration = -1, float playbackSpeed = -1f)
        {
            ICharSequence name = new Java.Lang.String(Application.Context.ApplicationInfo.Name);
            var notificationChannel = new NotificationChannel(channelId, name, NotificationImportance.Low);
            notificationChannel.SetSound(null, null);
            notificationManager.CreateNotificationChannel(notificationChannel);

            CreateActions();
            CreateBuilder(mediaItem);

            notification = notificationBuilder.Build();
            Notify();
        }

        public void UpdateElapsed(double position, float playbackSpeed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates IsPlaying action on Notification and toggle dismissable status
        /// </summary>
        /// <param name="playbackSpeed">Unused for Android</param>
        public void UpdateIsPlaying(float playbackSpeed = -1)
        {

            var isPlaying = AndroidAudioPlayer.SharedInstance.IsPlaying;

            if (isPlaying)
            {
                notificationBuilder.SetOngoing(true);
                notificationBuilder.SetSmallIcon(Android.Resource.Drawable.IcMediaPlay);
            }
            else
            {
                notificationBuilder.SetOngoing(false);
                notificationBuilder.SetSmallIcon(Android.Resource.Drawable.IcMediaPause);
            }

            notification = notificationBuilder.Build();
            notification.Actions[1] = isPlaying ? pauseAction : playAction;
            Notify();
        }

        internal void Notify()
        {
            notificationManager.Notify(0, notification);
        }

        public void Dispose()
        {
            notificationManager.Cancel(0);
        }

        private void CreateActions()
        {
            var context = Application.Context;

            var previousIntent = new Intent(ButtonEvents.AudioControlsPrevious);
            var previousPendingIntent = PendingIntent.GetBroadcast(context, 1, previousIntent, 0);
            previousAction = new Notification.Action(Android.Resource.Drawable.IcMediaPrevious, string.Empty, previousPendingIntent);

            var nextIntent = new Intent(ButtonEvents.AudioControlsNext);
            var nextPendingIntent = PendingIntent.GetBroadcast(context, 1, nextIntent, 0);
            nextAction = new Notification.Action(Android.Resource.Drawable.IcMediaNext, string.Empty, nextPendingIntent);

            var skipBackwardIntent = new Intent(ButtonEvents.AudioControlsSkipBackward);
            var skipBackwardPendingIntent = PendingIntent.GetBroadcast(context, 1, skipBackwardIntent, 0);
            skipBackwardAction = new Notification.Action(Android.Resource.Drawable.IcMediaRew, string.Empty, skipBackwardPendingIntent);

            var skipForwardIntent = new Intent(ButtonEvents.AudioControlsSkipForward);
            var skipForwardPendingIntent = PendingIntent.GetBroadcast(context, 1, skipForwardIntent, 0);
            skipForwardAction = new Notification.Action(Android.Resource.Drawable.IcMediaFf, string.Empty, skipForwardPendingIntent);

            var playIntent = new Intent(ButtonEvents.AudioControlsPlay);
            var playPendingIntent = PendingIntent.GetBroadcast(context, 1, playIntent, 0);
            playAction = new Notification.Action(Android.Resource.Drawable.IcMediaPlay, string.Empty, playPendingIntent);

            var pauseIntent = new Intent(ButtonEvents.AudioControlsPause);
            var pausePendingIntent = PendingIntent.GetBroadcast(context, 1, pauseIntent, 0);
            pauseAction = new Notification.Action(Android.Resource.Drawable.IcMediaPause, string.Empty, pausePendingIntent);

            var destroyIntent = new Intent(ButtonEvents.AudioControlsDestroy);
            var destroyPendingIntent = PendingIntent.GetBroadcast(context, 1, destroyIntent, 0);
            destroyAction = new Notification.Action(Android.Resource.Drawable.IcMenuCloseClearCancel, string.Empty, destroyPendingIntent);
        }

        private void CreateBuilder(IMediaItem mediaItem)
        {
            var builder = new Notification.Builder(Application.Context, channelId);

            builder.SetChannelId(channelId);

            //Configure builder
            builder.SetContentTitle(mediaItem.Title);
            builder.SetContentText(mediaItem.Artist);
            builder.SetWhen(0);

            if (AndroidAudioPlayer.SharedInstance.IsPlaying)
            {
                builder.SetOngoing(true);
                builder.SetSmallIcon(Android.Resource.Drawable.IcMediaPlay);
            }
            else
            {
                builder.SetOngoing(false);
                Intent dismissIntent = new Intent(ButtonEvents.AudioControlsDestroy);
                PendingIntent dismissPendingIntent = PendingIntent.GetBroadcast(Application.Context, 1, dismissIntent, 0);
                builder.SetDeleteIntent(dismissPendingIntent);
                builder.SetSmallIcon(Android.Resource.Drawable.IcMediaPause);
            }

            //If 5.0 >= set the controls to be visible on lockscreen
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                builder.SetVisibility(NotificationVisibility.Public);
            }

            if (!string.IsNullOrWhiteSpace(mediaItem.AlbumArtUrl))
            {
                try
                {
                    var bitmap = new AsyncImageDownloader().Execute(new string[] { mediaItem.AlbumArtUrl });
                    builder.SetLargeIcon(bitmap.GetResult());
                }
                catch (Java.Lang.Exception exc)
                {
                    Console.WriteLine($"Unable to find Album Art URL for {mediaItem.AlbumArtUrl}");
                    Console.WriteLine(exc.ToString());
                }
            }

            Intent resultIntent = new Intent(Application.Context, typeof(Activity));
            resultIntent.SetAction(Intent.ActionMain);
            resultIntent.AddCategory(Intent.CategoryLauncher);
            PendingIntent resultPendingIntent = PendingIntent.GetActivity(Application.Context, 0, resultIntent, PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(resultPendingIntent);

            //Controls
            var controlsCount = 0;

            // Previous
            controlsCount++;
            builder.AddAction(previousAction);

            // Play/Pause
            builder.AddAction(pauseAction);

            // Next
            controlsCount++;
            builder.AddAction(nextAction);

            // Close
            //controlsCount++;
            //builder.AddAction(destroyAction);

            // If 5.0 >= use MediaStyle
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                int[] args = new int[controlsCount];
                for (int i = 0; i < controlsCount; ++i)
                {
                    args[i] = i;
                }
                var style = new Notification.MediaStyle();
                style.SetShowActionsInCompactView(args);
                builder.SetStyle(style);
            }
            notificationBuilder = builder;
        }
    }
}
