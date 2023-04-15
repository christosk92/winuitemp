using LiteDB;

namespace Wavee.UI.Tracks.Models
{
    public class LocalTrack
    {
        [BsonId]
        public required string Path { get; init; }

        public required DateTime LastWrite { get; set; }
        public required string Title { get; set; }
        public required string[] Performers { get; set; }
        public required string[] PerformersRole { get; set; }
        public required string[] AlbumArtists { get; set; }
        public required string[] Composers { get; set; }
        public required string Album { get; set; }
        public required string Comment { get; set; }
        public required string[] Genres { get; set; }
        public required uint Year { get; set; }
        public required uint Track { get; set; }
        public required uint TrackCount { get; set; }
        public required uint Disc { get; set; }
        public required uint DiscCount { get; set; }
        public required uint Bpm { get; set; }
        public required string Conductor { get; set; }
        public required string Publisher { get; set; }
        public required int? ImageId { get; set; }
    }
}
