using System;

namespace CrossAudioManager
{
    public class LifeCycleEvents
    {
        public Action BeforePlaying { get; set; }
        public Action AfterPlaying { get; set; }
        public Action<PositionChangedEventArgs> PositionChanged { get; set; }
        public Action<BufferedChangedEventArgs> BufferedChanged { get; set; }
        public Action<IMediaItem> MediaItemChanged { get; set; }
        public Action<IMediaItem> MediaItemFinished { get; set; }
        public Action<PlaybackStateChangedEventArgs> PlaybackStateChanged { get; set; }
    }   
}
