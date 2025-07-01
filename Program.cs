class Program
{   
    static void Main(string[] args)
    {
        var chip8 = new Chip8(); // Создание экземпляра эмулятора
        chip8.LoadRom("Roms/Connect4.ch8"); // Загрузка ROM-файла (игра/программа в формате CHIP-8)
        chip8.Run();  // Запуска основного цикла эмуляции
    }
}
