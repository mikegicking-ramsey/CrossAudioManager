using System;

namespace CrossAudioManager
{
    public static class CrossAudioManager
    {
        internal static Lazy<IAudioManager> implementation = new Lazy<IAudioManager>(() => CreateAudioManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static bool IsSupported => implementation.Value != null;

        public static IAudioManager Current
        {
            get
            {
                var value = implementation.Value;
                if(value == null)
                {
                    throw new NotImplementedException();
                }
                return value;
            }
        }

        internal static IAudioManager CreateAudioManager()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
#pragma warning disable IDE0022
            return new AudioManagerImplementation();
#pragma warning restore IDE0022
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
