using Raylib_cs;

public class Menu
{
    private const string RomPath = "Roms/"; // Путь к папке
    
    private const string Extension = "ch8"; // Расширение файла
    
    /// <summary>
    /// Возвращает путь выбранного ROM-а
    /// </summary>
    /// <returns></returns>
    public string SelectMenu() 
    {
        Directories();
        List<string> aviableRoms = GetAviableRoms();
        int selected = 0;

        while (!Raylib.WindowShouldClose())
        {
            // Ввод
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selected = (selected + 1) % aviableRoms.Count;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selected = (selected - 1 + aviableRoms.Count) % aviableRoms.Count;
            if (Raylib.IsKeyPressed(KeyboardKey.Enter)) break;

            // Отрисовка
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            Raylib.DrawText("Select ROM", 10, 10, 20, Color.Yellow);

            for (int i = 0; i < aviableRoms.Count; i++) {
                var color = (i == selected) ? Color.Red : Color.White;
                Raylib.DrawText(aviableRoms[i], 20, 50 + i * 30, 20, color);
            }

            Raylib.EndDrawing();
        }
        
        return RomPath + aviableRoms[selected] + "." + Extension;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static void Directories()
    {
        try
        {
            if (Directory.Exists(RomPath))
            {
                string[] folders = Directory.GetDirectories(
                    Path.Combine(Directory.GetCurrentDirectory(), RomPath)
                );

                foreach (string folder in folders) {
                    Console.WriteLine(Path.GetFileName(folder));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Возвращает список доступных программ для запуска
    /// </summary>
    /// <returns></returns>
    private List<string> GetAviableRoms() 
    {
        List<string> aviableRoms = new List<string>();
        string searchExtensionPattern = $"*.{"ch8"}";
        string[] filePaths = Directory.GetFiles(RomPath, searchExtensionPattern);

        if (filePaths.Length > 0) {
            foreach (string filePath in filePaths) {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                aviableRoms.Add(fileName);
            }
        }

        return aviableRoms;
    }
}
