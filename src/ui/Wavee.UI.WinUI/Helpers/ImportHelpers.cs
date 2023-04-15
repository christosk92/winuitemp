using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.UI.Xaml;
using Wavee.UI.Tracks;
using Wavee.UI.Tracks.Models;

namespace Wavee.UI.WinUI.Helpers
{
    internal static class ImportHelpers
    {
        private static FileExtensionContentTypeProvider _provider = new();
        public static string[] AllowedAudioTypes = new[]
        {
            "audio/ogg",
            "audio/wave",
            "audio/wav",
            "audio/mpeg",
            "audio/mp4"
        };

        public static async Task Import(DragEventArgs dragEvent,
            Action<LocalTrack> onImported)
        {
            //Both folders and files
            var files = await dragEvent.DataView.GetStorageItemsAsync();
            foreach (var file in files)
            {
                if (file is StorageFile f)
                {
                    if (_provider.TryGetContentType(f.Path, out var ct) &&
                        AllowedAudioTypes.Contains(ct))
                    {
                        var imported = await ImportTrack(f);
                        onImported(imported);
                    }
                }
                else if (file is StorageFolder folder)
                {
                    var filesInFolder = await folder.GetFilesAsync();
                    foreach (var fileInFolder in filesInFolder)
                    {
                        if (
                            _provider.TryGetContentType(fileInFolder.Path, out var ct) &&
                            AllowedAudioTypes.Contains(ct))
                        {
                            var imported = await ImportTrack(fileInFolder);
                            onImported(imported);
                        }
                    }
                }
            }
        }

        private static async Task<LocalTrack> ImportTrack(StorageFile file)
        {
            var exists = LocalTracksDb.Instance.Exists(file.Path);
            var lastWrite = File.GetLastWriteTimeUtc(file.Path);

            if (exists)
            {
                var changed = LocalTracksDb.Instance.Find(file.Path);
                if (changed.LastWrite == lastWrite)
                    return changed;
            }

            using var tag = TagLib.File.Create(file.Path);

            int? imageId = default;
            if (tag.Tag.Pictures?.FirstOrDefault() is { } img)
            {
                imageId = await ImageCache.Instance.Store(img.Data.Data, (int)img.Data.Checksum);
            }

            var track = new LocalTrack
            {
                Path = file.Path,
                LastWrite = lastWrite,
                Title = tag.Tag.Title,
                Performers = tag.Tag.Performers,
                PerformersRole = tag.Tag.PerformersRole,
                AlbumArtists = tag.Tag.AlbumArtists,
                Composers = tag.Tag.Composers,
                Album = tag.Tag.Album,
                Comment = tag.Tag.Comment,
                Genres = tag.Tag.Genres,
                Year = tag.Tag.Year,
                Track = tag.Tag.Track,
                TrackCount = tag.Tag.TrackCount,
                Disc = tag.Tag.Disc,
                DiscCount = tag.Tag.DiscCount,
                Bpm = tag.Tag.BeatsPerMinute,
                Conductor = tag.Tag.Conductor,
                Publisher = tag.Tag.Publisher,
                ImageId = imageId
            };

            LocalTracksDb.Instance.AddOrUpdate(track);
            return track;
        }
    }
}
