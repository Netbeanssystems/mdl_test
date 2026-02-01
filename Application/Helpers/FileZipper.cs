using Application.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
namespace Application.Helpers
{
    public static class FileZipper
    {
        public static byte[] CreateArchive(List<InMemoryFile> files)
        {
            using var archiveStream = new MemoryStream();
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                    using var zipStream = zipArchiveEntry.Open();
                    zipStream.Write(file.Content, 0, file.Content.Length);
                }
            }
            var archiveFile = archiveStream.ToArray();
            return archiveFile;
        }
        ////Trial of extraction
        //public static byte[] ExtractArchive(InMemoryFile file)
        //{
        //    using var archiveStream = new MemoryStream();
        //    using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, true))
        //    {
        //        var zipArchiveEntry = archive.GetEntry(file.FileName);
        //        using var zipStream = zipArchiveEntry.Open();
        //        zipStream.Write(file.Content, 0, file.Content.Length);
        //    }
        //    var archiveFile = archiveStream.ToArray();
        //    return archiveFile;
        //}
    }
}
