using System.IO;
using System.Threading.Tasks.Sources;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Wavee.UI
{
    public class ImageCache
    {
        private readonly IApplicationDataFolderProvider _applicationDataFolderProvider;
        public ImageCache(IApplicationDataFolderProvider applicationDataFolderProvider)
        {
            _applicationDataFolderProvider = applicationDataFolderProvider;
        }
        private static ImageCache? _instance;
        public static ImageCache Instance { get; } = _instance ??= Ioc.Default.GetRequiredService<ImageCache>();
        public string? Get(int id)
        {
            var localCache = _applicationDataFolderProvider.Path;
            var path = Path.Combine(localCache, "Images", id.ToString());
            if (File.Exists(path)) return path;
            return null;
        }

        public ValueTask<int> Store(byte[] vec, int checksum)
        {
            var localCache = _applicationDataFolderProvider.Path;
            var incompletepath = Path.Combine(localCache, "Images");
            Directory.CreateDirectory(incompletepath);
            var path = Path.Combine(incompletepath, checksum.ToString());
            if (File.Exists(path)) return new ValueTask<int>(checksum);
            return new ValueTask<int>(StoreInternal(checksum, path, vec));
        }

        public async Task<int> StoreInternal(int id, string path, byte[] vec)
        {
            await File.WriteAllBytesAsync(path, vec);
            return id;
        }
    }

    public interface IApplicationDataFolderProvider
    {
        string Path { get; }
    }
}
