using Raylib_cs;

public class Chip8
{
    private CPU cpu;                       // Центральный процессор (обрабатывает инструкции)
    private Memory memory;                 // Память (4КБ)
    private Display display;               // Дисплей (64x32 пикселя)
    private Keyboard keyboard;             // Клавиатура (16 клавиш)
    
    public Chip8()
    {
        memory = new Memory();             // Инициализация памяти
        display = new Display();           // Инициализация дисплея
        keyboard = new Keyboard();         // Инициализация клавиатуры
        cpu = new CPU(memory, display, keyboard); // Создание процессора, передавая ему ссылки на остальные компоненты
    }

    /// <summary>
    /// Возвращает наименование выбранного ROM-а
    /// </summary>
    /// <returns></returns>
    public void Menu() 
    {
        var menu = new Menu();
        string rom = menu.Chip8Menu();
        LoadRom(rom); // Загрузка ROM-файла (игра/программа в формате CHIP-8)
    }
    
    /// <summary>
    /// Загрузка Rom-файла игры в память 
    /// </summary>
    /// <param name="path">Путь к Rom файлу</param>
    public void LoadRom(string path)
    {
        if (!File.Exists(path)) {
            Console.WriteLine("ROM-файл не найден: " + Path.Combine(Directory.GetCurrentDirectory(), path));
            return;
        }
        
        var data = File.ReadAllBytes(path); // Чтение содержимого ROM-файла в байтовый массив
        memory.LoadProgram(data); // Загрузка программы в память, начиная с адреса 0x200
    }
    
    /// <summary>
    /// Основной метод программы, цикл вызова
    /// </summary>
    public void Run()
    {
        try
        {
            while (!Raylib.WindowShouldClose()) {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                
                cpu.Cycle();
                
                if (cpu.isDrawFlagRender)
                {
                    if (cpu.isDrawFlagRender) {
                        display.Draw();
                        cpu.drawFlag = false;
                    }
                } else {
                    display.Draw();
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        } catch (Exception ex) {
            Console.WriteLine("Ошибка во время выполнения: " + ex.Message);
        }
    }
}
