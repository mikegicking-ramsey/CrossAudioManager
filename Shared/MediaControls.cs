using System;

namespace CrossAudioManager
{
    public class MediaControls : IMediaControls
    {
        public Action SkipBackward { get; set; }
        public Action SkipForward { get; set; }
        public Action SeekBackward { get; set; }
        public Action SeekForward { get; set; }
        public Action Stop { get; set; }
        public Action Pause { get; set; }
        public Action Play { get; set; }
        public Action PlayPause { get; set; }
        public Action Previous { get; set; }
        public Action Next { get; set; }
        public Action Shuffle { get; set; }
        public Action Repeat { get; set; }
        public Func<object, bool> MediaButtonEvent { get; set; }
    }
}
