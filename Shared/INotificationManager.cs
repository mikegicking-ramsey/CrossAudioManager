using System;

namespace CrossAudioManager
{
    public interface INotificationManager
    {
        void BuildNotification(IMediaItem mediaItem, double position = -1, double duration = -1, float playbackSpeed = 0f);
        void UpdateElapsed(double position, float playbackSpeed);
        void UpdateIsPlaying(float playbackSpeed = -1);
    }
}
