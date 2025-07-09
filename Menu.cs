using Raylib_cs;

public class Menu
{
    private const string RomPath = "Roms/"; // Путь к папке

    private const string Extension = "ch8"; // Расширение файла

    private const int RomsPerPage = 6; // Количество ROM'ов на странице
    
    private int _currentPage = 1; // Текущая страница
    private int _countPages = 1;

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
        if (selectedRom == -1)
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
        string[] currentItemsPage = CurrentPageRoms(items, shouldBackParentFolder);
        
        while (!Raylib.WindowShouldClose())
        {
            // Ввод
            if (Raylib.IsKeyPressed(KeyboardKey.Enter)) break;
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selected = (selected + 1) % currentItemsPage.Length;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selected = (selected - 1 + currentItemsPage.Length) % currentItemsPage.Length;
            
            if (Raylib.IsKeyPressed(KeyboardKey.Left)) {
                currentItemsPage = CurrentPageRoms(items, shouldBackParentFolder, "LEFT");
            }
            
            if (Raylib.IsKeyPressed(KeyboardKey.Right)) {
                currentItemsPage = CurrentPageRoms(items, shouldBackParentFolder, "RIGHT");
            }

            // Отрисовка
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            Raylib.DrawText(text, 10, 10, 20, Color.Yellow);

            if (shouldBackParentFolder) {
                Raylib.DrawText(
                    string.Format("Page {0} / {1}", _currentPage, _countPages),
                    510,
                    300,
                    20,
                    Color.Yellow
                );    
            }
            
            for (int i = 0; i < currentItemsPage.Length; i++)
            {
                var color = (i == selected) ? Color.Red : Color.White;
                Raylib.DrawText(currentItemsPage[i], 20, 50 + i * 30, 20, color);
            }
            
            Raylib.EndDrawing();
        }
        
        if (selected == (currentItemsPage.Length - 1)) {
            return -1;
        } else {
            if (_currentPage > 1) {
                return _currentPage * RomsPerPage + selected;
            } else {
                return selected;
            }
        }
    }

    /// <summary>
    /// Получает ROM'ы текущей страницы
    /// </summary>
    /// <param name="items"></param>
    /// <param name="shouldBackParentFolder"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private string[] CurrentPageRoms(List<string> items, bool shouldBackParentFolder, string type = "") {
        
        int itemsCount = items.Count;

        // Возврат к выбору папок
        if (shouldBackParentFolder) itemsCount++;
        _countPages = itemsCount / RomsPerPage;
        
        if (_countPages == 0) _countPages = 1;

        if (type == "RIGHT") {
            _currentPage++;
            if (_currentPage > _countPages)  _currentPage = 1;
        } else if (type == "LEFT") {
            _currentPage--;
            if (1 >= _currentPage) _currentPage = _countPages;
        } else {
            _currentPage = 1;
        }

        // Пропуск ROM'ов предыдущих страниц
        int skip = 0;
        if (_currentPage > 1) {
            skip = _currentPage * RomsPerPage;
        }
        
        string[] currentItemsPage = [];
        
        currentItemsPage = items.Skip(skip).Take(RomsPerPage).ToArray();
        return currentItemsPage.Append(shouldBackParentFolder ? "Back" : "Exit").ToArray();
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
                folders.Sort();
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
            }
        }

        return availableRoms;
    }

    /// <summary>
    /// Загрузка
    /// </summary>
    private void Loading(string message = "Loading")
    {
        _currentPage = 1;
        while (Raylib.IsKeyDown(KeyboardKey.Enter))
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText(message, 10, 10, 20, Color.Gray);
            Raylib.EndDrawing();
        }
    }
}
