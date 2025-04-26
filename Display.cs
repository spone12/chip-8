using Raylib_cs;

public class Display
{
    public const ushort ScreenWidth = 64;            // Ширина дисплея CHIP-8
    public const ushort ScreenHeight = 32;           // Высота дисплея CHIP-8
    public const ushort PixelSize = 10;           // Размер пикселей
    public const ushort FPS = 100;

    public bool[,] pixels = new bool[ScreenWidth, ScreenHeight]; // Массив для хранения состояния пикселей

    public Display() {
        Raylib.InitWindow(ScreenWidth * PixelSize, ScreenHeight * PixelSize, "CHIP-8");
        Raylib.InitAudioDevice();
        Raylib.SetTargetFPS(FPS);
    }
    public void Clear()
    {
        Array.Clear(pixels, 0, pixels.Length); // Очистка экрана (все пиксели в false)
    }

    public void Draw() {
  
        // Рендер пикселей
        for (ushort y = 0; y < ScreenHeight; y++) {
            for (ushort x = 0; x < ScreenWidth; x++) {
                if (pixels[x, y]) {
                    Raylib.DrawRectangle(x * PixelSize, y * PixelSize, PixelSize, PixelSize, Color.Green);
                }
            }
        }
    }

    public bool SetPixels(ushort opcode, byte[] V, ushort I, Memory memory)
    {
        byte x = (byte)((opcode & 0x0F00) >> 8);
        byte y = (byte)((opcode & 0x00F0) >> 4);
        byte height = (byte)(opcode & 0x000F);

        bool collision = false; // Столкновение пикселей
        
        // Координаты начала рисования на экране
        // Получаем значение из регистра V[x] и V[y], применяем модуль по ширине и высоте, чтобы не вылезать за края экрана
        int startX = V[x] % ScreenWidth;
        int startY = V[y] % ScreenHeight;
        
        // Каждая строка 1 байт из памяти
        for (int row = 0; row < height; row++) {
            byte spriteByte = memory[I + row];
            
            // В каждой строке ровно 8 битов => 8 пикселей по горизонтали.
            for (int col = 0; col < 8; col++) {
                
                // Проверяем конкретный бит внутри байта
                // 10000000 >> col 
                if ((spriteByte & (0x80 >> col)) != 0) {
                    
                    // Вычисление реальной позиции пикселя на экране.
                    int px = (startX + col) % ScreenWidth;
                    int py = (startY + row) % ScreenHeight;
                    
                    // Если пиксель был уже включен (true), ставим флаг collision = true.
                    if (pixels[px, py]) collision = true;
                    // XOR: инвертируем пиксель:
                    pixels[px, py] ^= true;
                }
            }
        }
        
        // Возвращение результата: было ли в процессе рисования столкновение
        return collision;
    }

    
    public void PrintToConsole()
    {
        for (ushort y = 0; y < ScreenHeight; y++) {
            string line = "";
            
            for (ushort x = 0; x < ScreenWidth; x++) {
                line += pixels[x, y] ? "█" : " ";
            }
            Console.WriteLine(line);
            Thread.Sleep(5);
        }
    }
}
