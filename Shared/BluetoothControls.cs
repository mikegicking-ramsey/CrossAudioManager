using System;

namespace CrossAudioManager
{
    public class BluetoothControls : IBluetoothControls
    {
        public Action Play { get; set; }
        public Action Pause { get; set; }
        public Action SeekForward { get; set; }
        public Action StepForward { get; set; }
        public Action SeekBackward { get; set; }
        public Action StepBackward { get; set; }
    }
}
