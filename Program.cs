using Utility_for_reading_and_saving_files.Files;
using Utility_for_reading_and_saving_files.Files.Factory;


string binFolder = AppContext.BaseDirectory;
string dataFolder = Path.Combine(binFolder, "data");
string extractedFolder = Path.Combine(binFolder, "extracted");

Directory.CreateDirectory(dataFolder);

Directory.CreateDirectory(extractedFolder);


string filePath = Path.Combine(dataFolder, "example.txt");
string filePathArchive = Path.Combine(dataFolder, "example.rar");
string archiveType = "rar";
string content = "This is an example content for the file.";


IFileManagement fileManagement = new FileManagement();
fileManagement.WriteFileAsync(filePath, content, archiveType).GetAwaiter().GetResult();
Console.WriteLine(fileManagement.ReadFileAsync(filePath).GetAwaiter().GetResult());


var factory = new ConcreteArchiveFactory();
Archive archive = factory.CreateArchive(archiveType);
archive.ExtractArchive(filePathArchive, extractedFolder).GetAwaiter().GetResult();

Console.WriteLine($"Files extracted to: {extractedFolder}");
