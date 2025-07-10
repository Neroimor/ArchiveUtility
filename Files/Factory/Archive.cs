using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;


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

        private const string RarExe = "C:\\Program Files\\WinRAR\\rar.exe";

        public override async Task<bool> GoToArchive(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                Console.WriteLine($"Source not found: {sourcePath}");
                return false;
            }

            string archivePath = Path.ChangeExtension(sourcePath, ".rar");
            string sourceDir = Path.GetDirectoryName(sourcePath)!;
            string fileName = Path.GetFileName(sourcePath);

            var psi = new ProcessStartInfo
            {
                FileName = RarExe,
                WorkingDirectory = sourceDir,
                Arguments = $"a \"{archivePath}\" \"{fileName}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using var proc = Process.Start(psi);
                if (proc == null)
                {
                    Console.WriteLine("Failed to start rar.exe process.");
                    return false;
                }

                string output = await proc.StandardOutput.ReadToEndAsync();
                string error = await proc.StandardError.ReadToEndAsync();
                await proc.WaitForExitAsync();

                if (proc.ExitCode != 0)
                {
                    Console.WriteLine($"RAR error (code {proc.ExitCode}): {error}");
                    return false;
                }

                Console.WriteLine($"Created RAR archive: {archivePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running rar.exe: {ex.Message}");
                return false;
            }
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
