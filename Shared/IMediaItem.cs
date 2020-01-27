using System;

namespace CrossAudioManager
{
    public interface IMediaItem
    {
        string MediaUrl { get; set; }
        string Id { get; set; }
        string Title { get; set; }
        string Artist { get; set; }
        string Album { get; set; }
        string AlbumArtUrl { get; set; }
    }
}
