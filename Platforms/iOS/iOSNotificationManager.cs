using System;
using MediaPlayer;

namespace CrossAudioManager.Platforms.iOS
{
    public class iOSNotificationManager : INotificationManager
    {
        private static Lazy<INotificationManager> notificationManager = new Lazy<INotificationManager>(() => new iOSNotificationManager());
        public static INotificationManager SharedInstance => notificationManager.Value;
        private MPNowPlayingInfoCenter mPNowPlayingInfoCenter;

        public iOSNotificationManager()
        {
            mPNowPlayingInfoCenter = MPNowPlayingInfoCenter.DefaultCenter;
        }

        public void BuildNotification(IMediaItem mediaItem, double position, double duration, float playbackSpeed)
        {
            if (mediaItem == null)
                return;

            if (double.IsNaN(position))
                position = 0;

            var nowPlayingInfo = new MPNowPlayingInfo()
            {
                Title = mediaItem.Title,
                AlbumTitle = mediaItem.Album,
                Artist = mediaItem.Artist,
                ElapsedPlaybackTime = position,
                PlaybackDuration = duration,
                PlaybackRate = playbackSpeed
            };

            mPNowPlayingInfoCenter.NowPlaying = nowPlayingInfo;
        }

        public void UpdateElapsed(double position, float playbackSpeed)
        {
            if (mPNowPlayingInfoCenter.NowPlaying == null)
                return;

            if (double.IsNaN(position))
                position = 0;

            var nowPlayingInfo = mPNowPlayingInfoCenter.NowPlaying;
            var newPlayingInfo = new MPNowPlayingInfo()
            {
                Title = nowPlayingInfo.Title,
                AlbumTitle = nowPlayingInfo.AlbumTitle,
                Artist = nowPlayingInfo.Artist,
                ElapsedPlaybackTime = position,
                PlaybackDuration = nowPlayingInfo.PlaybackDuration,
                PlaybackRate = playbackSpeed

            };
            mPNowPlayingInfoCenter.NowPlaying = newPlayingInfo;
        }

        public void UpdateIsPlaying(float playbackSpeed)
        {
            if (mPNowPlayingInfoCenter.NowPlaying == null)
                return;

            var nowPlayingInfo = mPNowPlayingInfoCenter.NowPlaying;
            var newPlayingInfo = new MPNowPlayingInfo()
            {
                Title = nowPlayingInfo.Title,
                AlbumTitle = nowPlayingInfo.AlbumTitle,
                Artist = nowPlayingInfo.Artist,
                ElapsedPlaybackTime = nowPlayingInfo.ElapsedPlaybackTime,
                PlaybackDuration = nowPlayingInfo.PlaybackDuration,
                PlaybackRate = playbackSpeed

            };
            mPNowPlayingInfoCenter.NowPlaying = newPlayingInfo;
        }

        public void UpdateNotification(IMediaItem mediaItem)
        {
            throw new NotImplementedException("This is an Android only method");
        }
    }
}
