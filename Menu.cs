using Raylib_cs;

public class Menu
{
    private const string RomPath = "Roms/"; // Путь к папке

    private const string Extension = "ch8"; // Расширение файла

    /// <summary>
    /// Возвращает путь выбранного ROM-а
    /// </summary>
    /// <returns></returns>
    public string Chip8Menu()
    {
        List<string> folders = GetRomFolders();
        int selectedFolder = SelectMenu(folders, "Select Folder");

        Loading();

        List<string> availableRoms = GetAvailableRoms(folders[selectedFolder]);
        int selectedRom = SelectMenu(availableRoms, "Select Rom", true);

        // Если выбран возврат к папкам
        if ((selectedRom + 1) == availableRoms.Count)
        {
            Loading();
            return Chip8Menu();
        }

        return Path.Combine(
            RomPath,
            folders[selectedFolder],
            Path.ChangeExtension(availableRoms[selectedRom], Extension)
        );
    }

    /// <summary>
    /// Возвращает Rom файлы и папки
    /// </summary>
    /// <param name="items"></param>
    /// <param name="text"></param>
    /// <param name="shouldBackParentFolder"></param>
    /// <returns></returns>
    private int SelectMenu(List<string> items, string text, bool shouldBackParentFolder = false)
    {
        int selected = 0;
        int itemsCount = items.Count;

        // Возврат к выбору папок
        if (shouldBackParentFolder)
        {
            itemsCount++;
            items.Add("Back");
        }

        while (!Raylib.WindowShouldClose())
        {
            // Ввод
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selected = (selected + 1) % items.Count;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selected = (selected - 1 + items.Count) % items.Count;
            if (Raylib.IsKeyPressed(KeyboardKey.Enter)) break;

            // Отрисовка
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            Raylib.DrawText(text, 10, 10, 20, Color.Yellow);

            for (int i = 0; i < itemsCount; i++)
            {
                var color = (i == selected) ? Color.Red : Color.White;
                Raylib.DrawText(items[i], 20, 50 + i * 30, 20, color);
            }

            Raylib.EndDrawing();
        }
        return selected;
    }

    /// <summary>
    /// Возвращает список доступных папок
    /// </summary>
    /// <returns></returns>
    private List<string> GetRomFolders()
    {
        List<string> folders = new List<string>();

        try
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), RomPath);

            if (Directory.Exists(directory))
            {
                foreach (string folder in Directory.GetDirectories(directory))
                {
                    folders.Add(Path.GetFileName(folder));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        return folders;
    }


    /// <summary>
    /// Возвращает список доступных программ для запуска
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    private List<string> GetAvailableRoms(string folder)
    {
        List<string> availableRoms = new List<string>();
        string searchExtensionPattern = $"*.{"ch8"}";
        string[] filePaths = Directory.GetFiles(
            Path.Combine(RomPath, folder),
            searchExtensionPattern
        );

        if (filePaths.Length > 0)
        {
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                availableRoms.Add(fileName);

                Console.WriteLine(filePath);
            }
        }

        return availableRoms;
    }

    /// <summary>
    /// Загрузка
    /// </summary>
    private void Loading(string message = "Loading")
    {
        while (Raylib.IsKeyDown(KeyboardKey.Enter))
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText(message, 10, 10, 20, Color.Gray);
            Raylib.EndDrawing();
        }
    }
}
