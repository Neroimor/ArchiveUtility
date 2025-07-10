using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_for_reading_and_saving_files.Files.Factory;

namespace Utility_for_reading_and_saving_files.Files
{
    internal class FileManagement : IFileManagement
    {
        public async Task<string> ReadFileAsync(string filePath)
        {
            string content = string.Empty;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string fileContent = await reader.ReadToEndAsync();
                content = fileContent;
            }

            return content;

        }
        public async Task<bool> WriteFileAsync(string filePath, string content, string archiveType)
        {

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    await writer.WriteAsync(content);
                }
                if (!string.IsNullOrEmpty(archiveType))
                {
                    ArchiveFactory archiveFactory = new ConcreteArchiveFactory();
                    Archive archive = archiveFactory.CreateArchive(archiveType);
                    return await archive.GoToArchive(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
                return false;
            }
        }
    }
    
    
}
