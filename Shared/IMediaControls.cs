using System;

namespace CrossAudioManager
{
    public interface IMediaControls
    {
        /// <summary>
        /// If null, default action is to step backwards by MediaManager.StepSize (default 15 seconds)
        /// </summary>
        Action SkipBackward { get; set; }

        /// <summary>
        /// If null, default action is to step forwards by MediaManager.StepSize (default 15 seconds)
        /// </summary>
        Action SkipForward { get; set; }

        /// <summary>
        /// If null, default action is to step backwards by MediaManager.StepSize (default 15 seconds)
        /// </summary>
        Action SeekBackward { get; set; }

        /// <summary>
        /// If null, default action is to step forwards by MediaManager.StepSize (default 15 seconds)
        /// </summary>
        Action SeekForward { get; set; }

        /// <summary>
        /// If null, default action is to stop playback
        /// </summary>
        Action Stop { get; set; }

        /// <summary>
        /// If null, default action is to pause playback
        /// </summary>
        Action Pause { get; set; }

        /// <summary>
        /// If null, default action is to resume or start playback
        /// </summary>
        Action Play { get; set; }

        /// <summary>
        /// If null, default action is to toggle play/pause
        /// </summary>
        Action PlayPause { get; set; }

        /// <summary>
        /// If null, default action is to start playback of the previous media item in the queue
        /// </summary>
        Action Previous { get; set; }

        /// <summary>
        /// If null, default action is to start playback of the next media item in the queue
        /// </summary>
        Action Next { get; set; }

        /// <summary>
        /// If null, default action is to toggle the shuffle flag
        /// </summary>
        Action Shuffle { get; set; }

        /// <summary>
        /// If null, default action is to toggle the repeat flag
        /// </summary>
        Action Repeat { get; set; }

        /// <summary>
        /// Overrides the OnMediaButtonEvent Handler for Android. For iOS, please use the other fields in this class.
        /// <para>
        /// intent: The Android.Content.Intent of the MediaButtonEvent
        /// </para>
        /// </summary>
        /// <remarks>
        /// The OnMediaButtonEvent does not exist for iOS.
        /// </remarks>
        Func<object, bool> MediaButtonEvent { get; set; }
    }
}
