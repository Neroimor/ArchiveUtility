using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;


namespace Utility_for_reading_and_saving_files.Files.Factory
{
    internal abstract class Archive
    {
        public abstract Task<bool> GoToArchive(string sourcePath);

        public abstract Task<bool> ExtractArchive(string archivePath, string destinationPath);
    }

    internal class ConcreateArchiveZIP : Archive
    {
        public override async Task<bool> GoToArchive(string sourcePath)
        {
            if (!Directory.Exists(sourcePath) && !File.Exists(sourcePath))
            {
                Console.WriteLine($"Source not found: {sourcePath}");
                return false;
            }

            var archivePath = Path.ChangeExtension(sourcePath, ".zip");
            await Task.Run(() =>
            {

                if (File.Exists(archivePath)) File.Delete(archivePath);
                if (Directory.Exists(sourcePath))
                {
                    ZipFile.CreateFromDirectory(sourcePath, archivePath, CompressionLevel.Optimal, includeBaseDirectory: false);
                }
                else
                {
                    using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create);
                    archive.CreateEntryFromFile(sourcePath, Path.GetFileName(sourcePath));
                }
                Console.WriteLine($"Created ZIP archive: {archivePath}");
            });
            return true;
        }

        public override async Task<bool> ExtractArchive(string archivePath, string destinationPath)
        {
            if (!File.Exists(archivePath))
            {
                Console.WriteLine($"ZIP archive not found: {archivePath}");
                return false;
            }

            await Task.Run(() =>
            {
                Directory.CreateDirectory(destinationPath);
                ZipFile.ExtractToDirectory(archivePath, destinationPath, overwriteFiles: true);
                Console.WriteLine($"Extracted ZIP archive from {archivePath} to {destinationPath}");
            });
            return true;
        }
    }

    internal class ConcreateArchiveRAR : Archive
    {
        public override async Task<bool> GoToArchive(string sourcePath)
        {
            Console.WriteLine("RAR creation is not supported by SharpCompress. Please use ZIP or an external RAR tool.");
            return await Task.FromResult(false);
        }

        public override async Task<bool> ExtractArchive(string archivePath, string destinationPath)
        {
            if (!File.Exists(archivePath))
            {
                Console.WriteLine($"RAR archive not found: {archivePath}");
                return false;
            }

            await Task.Run(() =>
            {
                Directory.CreateDirectory(destinationPath);
                using var archive = RarArchive.Open(archivePath);
                archive.WriteToDirectory(destinationPath, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
                Console.WriteLine($"Extracted RAR archive from {archivePath} to {destinationPath}");
            });
            return true;
        }
    }

    internal abstract class ArchiveFactory
    {
        public abstract Archive CreateArchive(string archiveType);
    }

    internal class ConcreteArchiveFactory : ArchiveFactory
    {
        public override Archive CreateArchive(string archiveType)
        {
            return archiveType.ToLower() switch
            {
                "zip" => new ConcreateArchiveZIP(),
                "rar" => new ConcreateArchiveRAR(),
                _ => throw new NotSupportedException($"Archive type {archiveType} is not supported.")
            };
        }
    }
}
