using System;
using System.Timers;
using AVFoundation;
using Foundation;
using MediaPlayer;
using UIKit;

namespace CrossAudioManager.Platforms.iOS.Playback
{
    public class AudioPlayer : IAudioPlayer, IDisposable
    {
        private static Lazy<IAudioPlayer> _audioPlayer = new Lazy<IAudioPlayer>(() => new AudioPlayer());
        public static IAudioPlayer SharedInstance => _audioPlayer.Value;

        public PlayerConfig Configuration { get; set; }
        public IMediaControls MediaControls { get; set; }
        public INotificationManager NotificationManager { get; set; }

        public int TimeScale = 60;
        public double PositionTimerInterval { get; set; } = 1000;
        public float PlaybackSpeed { get; set; } = 1.0f;
        public AudioAction LastAudioAction { get; set; } = AudioAction.None;



        #region Audio Playback Properties

        private AVPlayer audioPlayer;
        private AVAsset currentAsset;
        private Timer positionTimer;
        private AVAudioSession audioSession;
        private const string playableKey = "playableKey";
        private string[] avKeys = new string[] { playableKey };

        #endregion



        #region LifeCycle Events

        public event EventHandler BeforePlaying;
        public event EventHandler AfterPlaying;
        public event PositionChangedEventHandler PositionChanged;
        public event BufferedChangedEventHandler BufferedChanged;
        public event MediaItemEventHandler MediaItemChanged;
        public event MediaItemEventHandler MediaItemFinished;
        public event PlaybackStateChangedEventHandler PlaybackStateChanged;

        #endregion

        #region State Events / Observers

        private NSObject didFinishPlayingObserver;
        private IDisposable loadedTimeRangesToken;
        private IDisposable statusToken;
        #endregion

        private MPNowPlayingInfoCenter mPNowPlayingInfoCenter;

        public double Position { get; set; }

        private double previousPosition;
        public double PreviousPosition
        {
            get => previousPosition;
            set
            {
                PositionChanged?.Invoke(this, new PositionChangedEventArgs() { PreviousPosition = previousPosition, Position = value });
                previousPosition = value;
            }
        }

        public double Duration { get; set; } = 0;

        public bool IsPlaying
        {
            get => audioPlayer != null && audioPlayer.Rate > 0;
        }

        public IMediaItem CurrentItem { get; set; }

        private double bufferedSeconds;
        public double BufferedSeconds
        {
            get =>
                bufferedSeconds;
            set
            {
                bufferedSeconds = value;
                BufferedChanged?.Invoke(this, new BufferedChangedEventArgs() { BufferedValue = value });
            }
        }

        public LifeCycleEvents LifeCycleEvents { get; set; }

        public AudioPlayer()
        {
            MediaControls = new MediaControls();
            Configuration = new PlayerConfig();
        }

        public void Initialize()
        {
            audioPlayer = new AVPlayer();
            var options = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New;
            loadedTimeRangesToken = audioPlayer.AddObserver("currentItem.loadedTimeRanges", options, LoadedTimeRangesChanged);
            statusToken = audioPlayer.AddObserver("status", options, StatusChanged);
            didFinishPlayingObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidFinishPlaying);
            mPNowPlayingInfoCenter = MPNowPlayingInfoCenter.DefaultCenter;

            NotificationManager = iOSNotificationManager.SharedInstance;

            audioPlayer.InvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });

            InitializeAudioSession();
            InitializePositionTimer();
        }

        private void StatusChanged(NSObservedChange obj)
        {
        }

        private void InitializeAudioSession()
        {
            audioSession = AVAudioSession.SharedInstance();
            audioSession.SetCategory(AVAudioSessionCategory.Playback);
        }

        private void InitializePositionTimer()
        {
            positionTimer = new Timer(PositionTimerInterval);
            positionTimer.Elapsed += OnPositionTimerElapsed;
            positionTimer.Start();
        }


        private void OnPositionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Position = audioPlayer == null || audioPlayer.CurrentTime.IsIndefinite ? 0 : audioPlayer.CurrentTime.Seconds;
            PreviousPosition = Position;
        }

        protected virtual void LoadedTimeRangesChanged(NSObservedChange obj)
        {
            double buffered = 0;
            if (audioPlayer?.CurrentItem != null && audioPlayer.CurrentItem.LoadedTimeRanges.Any())
            {
                buffered = audioPlayer.CurrentItem.LoadedTimeRanges.Max(ltr => ltr.CMTimeRangeValue.Start.Seconds + ltr.CMTimeRangeValue.Duration.Seconds);

            }
            BufferedSeconds = buffered;
        }

        protected virtual void DidFinishPlaying(NSNotification obj)
        {
            MediaItemFinished?.Invoke(this, new MediaItemEventArgs() { MediaItem = CurrentItem });
        }

        public void FastForward()
        {
            if (MediaControls.SeekForward != null)
            {
                MediaControls.SeekForward.Invoke();
            }
            else
            {
                SeekForward();
            }
        }

        public void Next()
        {
            if (MediaControls.Next != null)
            {
                MediaControls.Next.Invoke();
            }
            else
            {
                MediaControls.SeekForward();
            }
        }

        public void Pause()
        {
            positionTimer.Stop();
            if (MediaControls.Pause != null)
            {
                MediaControls.Pause.Invoke();
            }
            else
            {
                audioPlayer.Pause();
            }

            LastAudioAction = AudioAction.Pause;
            PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs() { IsPlaying = IsPlaying });
            NotificationManager.UpdateIsPlaying(0);
        }

        public void Play()
        {
            if (MediaControls.Play != null)
            {
                MediaControls.Play.Invoke();
            }
            else
            {
                audioPlayer.Play();
                SetRate();
            }

            LastAudioAction = AudioAction.Play;

            positionTimer.Start();
            PlaybackStateChanged?.Invoke(this, new PlaybackStateChangedEventArgs() { IsPlaying = IsPlaying });
            NotificationManager.UpdateIsPlaying(Configuration.PlaybackSpeed);
        }

        public void Play(string url)
        {
            BeforePlaying?.Invoke(this, new EventArgs());
            audioPlayer.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(new NSUrl(url)));
            MediaItemChanged?.Invoke(this, new MediaItemEventArgs() { MediaItem = null });
            Play();
            AfterPlaying?.Invoke(this, new EventArgs());
        }

        public void Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new EventArgs());
            CurrentItem = mediaItem;
            PrepareAVAsset(mediaItem);
            audioPlayer.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(currentAsset));
            MediaItemChanged?.Invoke(this, new MediaItemEventArgs() { MediaItem = mediaItem });
            Play();
            AfterPlaying?.Invoke(this, new EventArgs());
        }

        private void PrepareAVAsset(IMediaItem mediaItem)
        {
            currentAsset = AVAsset.FromUrl(new NSUrl(mediaItem.MediaUrl));
            currentAsset.LoadValuesAsynchronously(avKeys, () =>
            {
                NSError error = null;
                var status = currentAsset.StatusOfValue(playableKey, out error);

                switch (status)
                {
                    case AVKeyValueStatus.Loaded:
                        Duration = currentAsset.Duration.Seconds;
                        NotificationManager.BuildNotification(mediaItem, Position, Duration, audioPlayer.Rate);
                        break;
                    case AVKeyValueStatus.Failed:
                        //TODO: Throw error?
                        break;
                    case AVKeyValueStatus.Cancelled:
                        //TODO: Throw error?
                        break;
                }
            });
        }

        public void PlayPause()
        {
            if (MediaControls.PlayPause != null)
            {
                MediaControls.PlayPause.Invoke();
            }
            else
            {
                if (audioPlayer.Rate >= 1.0f)
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }
        }

        public void Previous()
        {
            if (MediaControls.Previous != null)
            {
                MediaControls.Previous.Invoke();
            }
            else
            {
                SeekBackward();
            }
        }

        public void Rewind()
        {
            if (MediaControls.SeekBackward != null)
            {
                MediaControls.SeekBackward.Invoke();
            }
            else
            {
                SeekBackward();
            }

        }

        public void SeekBackward()
        {
            if (MediaControls.SeekBackward != null)
            {
                MediaControls.SeekBackward.Invoke();
            }
            else
            {
                var targetTime = (int)audioPlayer.CurrentTime.Seconds - Configuration.StepSize;
                SeekTo(targetTime);
            }
        }

        public void SeekForward()
        {
            if (MediaControls.SeekForward != null)
            {
                MediaControls.SeekForward.Invoke();
            }
            else
            {
                var targetTime = (int)audioPlayer.CurrentTime.Seconds + Configuration.StepSize;
                SeekTo(targetTime);
            }
        }

        public void SeekTo(int seconds)
        {
            positionTimer.Stop();
            var targetTime = CMTime.FromSeconds(seconds, TimeScale);
            audioPlayer.Seek(targetTime, CMTime.Zero, CMTime.Zero);
            NotificationManager.UpdateElapsed(targetTime.Seconds, audioPlayer.Rate);
            positionTimer.Start();
        }

        public void SetRate(float rate)
        {
            Configuration.PlaybackSpeed = rate;
            SetRate();
        }

        public void SetRate()
        {
            audioPlayer.Rate = Configuration.PlaybackSpeed;
        }

        public void Stop()
        {
            LastAudioAction = AudioAction.Stop;
            audioPlayer.Pause();
        }

        public virtual void Dispose()
        {
            mPNowPlayingInfoCenter.NowPlaying = null;
            positionTimer.Elapsed -= OnPositionTimerElapsed;
            positionTimer.Dispose();
            audioPlayer.Dispose();
            loadedTimeRangesToken?.Dispose();
        }
    }
}
