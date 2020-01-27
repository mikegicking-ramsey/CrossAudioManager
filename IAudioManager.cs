using System;
namespace CrossAudioManager
{
    public interface IAudioManager
    {
        IBluetoothManager BluetoothManager { get; set; }
        IAudioPlayer AudioPlayer { get; set; }
    }
}
