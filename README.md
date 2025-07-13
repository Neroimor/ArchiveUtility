#﻿ Utility for Reading, Saving and Archiving Files

Небольшая консольная утилита на .NET для чтения/записи текстовых файлов и их автоматической упаковки в ZIP/RAR‑архив с помощью библиотеки [SharpCompress](https://github.com/adamhathcock/sharpcompress).

---

## 📋 Возможности

* **Чтение файла** (`ReadFileAsync`)
* **Запись файла** (`WriteFileAsync`) с опциональной архивацией
* **Создание ZIP/RAR‑архива** из файла или директории
* **Извлечение ZIP/RAR‑архива** в указанную папку
* Расширяемая архитектура через паттерн «Фабрика» для добавления новых форматов

---

## 🚀 Быстрый старт

1. **Клонируйте репозиторий**

   ```bash
   git clone https://github.com/your-username/utility-file-archiver.git
   cd utility-file-archiver
   ```

2. **Установите зависимости**

   ```bash
   dotnet tool restore
   dotnet add package SharpCompress
   ```

3. **Соберите проект**

   ```bash
   dotnet build
   ```

4. **Пример использования**

   ```csharp

	string binFolder = AppContext.BaseDirectory;
	string dataFolder = Path.Combine(binFolder, "data");
	string extractedFolder = Path.Combine(binFolder, "extracted");
	
	Directory.CreateDirectory(dataFolder);
	
	Directory.CreateDirectory(extractedFolder);
	
	
	string filePath = Path.Combine(dataFolder, "example.txt");
	string filePathArchive = Path.Combine(dataFolder, "example.zip");
	string archiveType = "zip";
	string content = "This is an example content for the file.";
	
	
	IFileManagement fileManagement = new FileManagement();
	fileManagement.WriteFileAsync(filePath, content, archiveType).GetAwaiter().GetResult();
	Console.WriteLine(fileManagement.ReadFileAsync(filePath).GetAwaiter().GetResult());
	
	
	var factory = new ConcreteArchiveFactory();
	Archive archive = factory.CreateArchive(archiveType);
	archive.ExtractArchive(filePathArchive, extractedFolder).GetAwaiter().GetResult();
	
	Console.WriteLine($"Files extracted to: {extractedFolder}");
   ```

---

## 🔧 Архитектура

1. **IFileManagement / FileManagement**

   * Методы `ReadFileAsync` и `WriteFileAsync`.
   * При записи создаётся файл, затем вызывается `ArchiveFactory` и выполняется упаковка.

2. **Archive (абстрактный класс)**

   * `GoToArchive(sourcePath)` — упаковка файла или папки.
   * `ExtractArchive(archivePath, destinationPath)` — распаковка архива.

3. **Реализации**

   * `ConcreteArchiveZIP` — ZIP через `System.IO.Compression`.
   * `ConcreteArchiveRAR` — распаковка RAR через SharpCompress.

4. **ArchiveFactory / ConcreteArchiveFactory**

   * По строковому ключу (`"zip"`, `"rar"`) возвращает нужную реализацию `Archive`.

---

## ⚙️ Конфигурация и расширение

### Добавление новых форматов

1. Создайте класс-наследник `Archive`.
2. Реализуйте методы `GoToArchive` и `ExtractArchive`.
3. Добавьте новый ключ в `ConcreteArchiveFactory.CreateArchive()`.

### .NET Target

Проект настроен на **.NET 9.0**.

---
