using System;

namespace CrossAudioManager
{
    public class AudioManagerBase : IAudioManager
    {
        public PlayerConfig Configuration { get; set; }

        public IBluetoothManager BluetoothManager { get; set; }
        public IAudioPlayer AudioPlayer { get; set; }
    }
}
