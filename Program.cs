// See https://aka.ms/new-console-template for more information
using Utility_for_reading_and_saving_files.Files;
using Utility_for_reading_and_saving_files.Files.Factory;


string binFolder = AppContext.BaseDirectory;
string dataFolder = Path.Combine(binFolder, "data");
string extractedFolder = Path.Combine(binFolder, "extracted");

// 1) Убедимся, что каталоги есть
Directory.CreateDirectory(dataFolder);

Directory.CreateDirectory(extractedFolder);

// 2) Пути к файлам
string filePath = Path.Combine(dataFolder, "example.txt");
string filePathArchive = Path.Combine(dataFolder, "example.zip");
string archiveType = "zip";
string content = "This is an example content for the file.";

// 3) Чтение/запись + архивация
IFileManagement fileManagement = new FileManagement();
fileManagement.WriteFileAsync(filePath, content, archiveType).GetAwaiter().GetResult();
Console.WriteLine(fileManagement.ReadFileAsync(filePath).GetAwaiter().GetResult());

// 4) Извлечение
var factory = new ConcreteArchiveFactory();
Archive archive = factory.CreateArchive(archiveType);
archive.ExtractArchive(filePathArchive, extractedFolder).GetAwaiter().GetResult();

Console.WriteLine($"Files extracted to: {extractedFolder}");
