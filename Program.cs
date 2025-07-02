class Program
{   
    static void Main(string[] args)
    {
        var chip8 = new Chip8(); // Создание экземпляра эмулятора
        chip8.Menu();
        chip8.Run();  // Запуска основного цикла эмуляции
    }
}
