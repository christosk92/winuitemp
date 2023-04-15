using LiteDB;
using Wavee.UI.Tracks.Models;

namespace Wavee.UI.Tracks
{
    public sealed class LocalTracksDb
    {
        public static LocalTracksDb Instance { get; } = new LocalTracksDb();

        private readonly ILiteDatabase _db;
        private readonly ILiteCollection<LocalTrack> _localTracks;
        public LocalTracksDb()
        {
            var toPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Wavee");
            Directory.CreateDirectory(toPath);


            var combined = Path.Combine(toPath, "localtracks.db");
            _db = new LiteDatabase(combined);
            _localTracks = _db.GetCollection<LocalTrack>();
        }

        public bool Exists(string path)
        {
            return _localTracks.Exists(x => x.Path == path);
        }

        public LocalTrack Find(string path)
        {
            return _localTracks.FindOne(x => x.Path == path);
        }

        public void AddOrUpdate(LocalTrack track)
        {
            _localTracks.Upsert(track);
        }

        public Task<IEnumerable<LocalTrack>> GetTracks()
        {
            return Task.FromResult(_localTracks
                .Query()
                .OrderBy(c => c.LastWrite)
                .ToEnumerable());
        }
    }
}
