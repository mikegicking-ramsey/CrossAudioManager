namespace CrossAudioManager
{
    public class MediaItem : IMediaItem
    {
        public MediaItem()
        {

        }

        public MediaItem(string url)
        {
            MediaUrl = url;
        }

        public string Id { get; set; }
        public string MediaUrl { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArtUrl { get; set; } = "https://cdn.ramseysolutions.net/media/b2c/broadcast/app/bg-lock-screen.jpg";
    }
}
