
public class Memory
{
    private byte[] memory = new byte[4096]; // 4КБ памяти

    // Индексатор: позволяет обращаться к памяти как к массиву, например memory[0x200]
    public byte this[int index]
    {
        get => memory[index];          // Получить байт по адресу
        set => memory[index] = value; // Установить байт по адресу
    }

    public void LoadProgram(byte[] program)
    {
        // Копирование программы в память, начиная с адреса 0x200 (512)
        Array.Copy(program, 0, memory, 0x200, program.Length);
    }
}