using System;

namespace CrossAudioManager
{
    public class PlayerConfig
    {
        /// <summary>
        /// The maximum value, in seconds, to seek forward/backward when a SeekForward/SeekBackward intent is received
        /// </summary>
        public int StepSize { get; set; }

        /// <summary>
        /// If flag is set to true, Next and Previous intents will trigure a step forward/backward instead of navigating the play queue
        /// </summary>
        public bool SeekInsteadOfSkip { get; set; }

        public float PlaybackSpeed { get; set; }
        
        public PlayerConfig()
        {
            StepSize = 15;
            PlaybackSpeed = 1.0f;
        }
    }
}
