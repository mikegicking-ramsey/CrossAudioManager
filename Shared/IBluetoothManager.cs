using System;

namespace CrossAudioManager
{
    public interface IBluetoothManager
    {
        IBluetoothControls BluetoothControls { get; set; }
        void Intialize();
        void Play();
        void Pause();
        void Rewind();
        void FastFoward();
        void ChangePosition(double newTime);
    }
}
