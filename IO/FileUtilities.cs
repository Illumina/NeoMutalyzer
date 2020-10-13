using System.IO;
using System.IO.Compression;

namespace IO
{
    public static class FileUtilities
    {
        public static FileStream GetReadStream(string path) =>
            new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        public static FileStream GetWriteStream(string path) => new FileStream(path, FileMode.Create);

        public static StreamReader StreamReader(string path) => new StreamReader(GetReadStream(path));
        public static StreamWriter StreamWriter(string path) => new StreamWriter(GetWriteStream(path));
        
        public static StreamReader GzipReader(string path) => new StreamReader(new GZipStream(GetReadStream(path), CompressionMode.Decompress));
        
        public static StreamWriter GzipWriter(string path) => new StreamWriter(new GZipStream(GetWriteStream(path), CompressionLevel.Optimal));
        
        public static ExtendedBinaryReader ExtendedBinaryReader(string path) => new ExtendedBinaryReader(GetReadStream(path));
    }
}