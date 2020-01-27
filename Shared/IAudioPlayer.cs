using System;

namespace CrossAudioManager
{
    public interface IAudioPlayer
    {
        IMediaItem CurrentItem { get; set; }

        INotificationManager NotificationManager { get; set; }

        double Position { get; }

        double PreviousPosition { get; set; }

        double Duration { get; set; }

        double BufferedSeconds { get; set; }

        /// <summary>
        ///     Timer interval to update the PreviousPosition, default 1 second
        /// </summary>
        double PositionTimerInterval { get; set; }

        /// <summary>
        ///  The last Audio Action performed by the Player
        /// </summary>
        AudioAction LastAudioAction { get; set; }

        /// <summary>
        ///  Overridable Media Controls for all Audio methods
        /// </summary>
        IMediaControls MediaControls { get; set; }

        /// <summary>
        ///     Initialize the Audio Player
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Resume playing the currently loaded media
        /// </summary>
        void Play();

        /// <summary>
        ///     Load and a play a media item by URL
        /// </summary>
        /// <param name="url"></param>
        void Play(string url);

        /// <summary>
        ///     Load and play a media item
        /// </summary>
        /// <param name="mediaItem"></param>
        void Play(IMediaItem mediaItem);

        /// <summary>
        ///     Pause the currently playing media
        /// </summary>
        void Pause();

        void Stop();

        /// <summary>
        ///     Toggle Play/Pause for the current media
        /// </summary>
        void PlayPause();

        /// <summary>
        ///     Skip ahead the specific Audio Increment in seconds
        /// </summary>
        void FastForward();

        /// <summary>
        ///     Skip ahead the specific Audio Increment in seconds
        /// </summary>
        void SeekForward();

        void Next();

        /// <summary>
        ///     Skip back the specific Audio Increment in seconds
        /// </summary>
        void Rewind();

        /// <summary>
        ///     Skip back the specific Audio Increment in seconds
        /// </summary>
        void SeekBackward();

        void Previous();

        /// <summary>
        ///     Seek to 
        /// </summary>
        /// <param name="seconds"></param>
        void SeekTo(int seconds);

        /// <summary>
        ///     Update CurrentPlayback speed and set the Rate
        /// </summary>
        /// <param name="rate"></param>
        void SetRate(float rate);

        /// <summary>
        ///     Set the playback speed rate to PlaybackSpeed
        /// </summary>
        void SetRate();

        #region Lifecycle Hooks

        event EventHandler BeforePlaying;

        event EventHandler AfterPlaying;

        event PositionChangedEventHandler PositionChanged;

        event BufferedChangedEventHandler BufferedChanged;

        event MediaItemEventHandler MediaItemChanged;

        event MediaItemEventHandler MediaItemFinished;

        event PlaybackStateChangedEventHandler PlaybackStateChanged;

        LifeCycleEvents LifeCycleEvents { get; set; }

        #endregion

        PlayerConfig Configuration { get; set; }

        bool IsPlaying { get; }
    }
}
