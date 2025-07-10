using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility_for_reading_and_saving_files.Files
{
    internal interface IFileManagement
    {

        public Task<string> ReadFileAsync(string filePath);
        public Task<bool> WriteFileAsync(string filePath, string content, string arhiveType);

    }
}
