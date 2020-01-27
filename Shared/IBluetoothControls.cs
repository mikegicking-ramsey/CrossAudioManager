using System;

namespace CrossAudioManager
{
    public interface IBluetoothControls
    {
        /// <summary>
        /// If null, default action is to resume or start playback
        /// </summary>
        Action Play { get; set; }

        /// <summary>
        /// If null, default action is to pause playback
        /// </summary>
        Action Pause { get; set; }

        Action SeekForward { get; set; }

        Action StepForward { get; set; }

        Action SeekBackward { get; set; }

        Action StepBackward { get; set; }
    }
}
