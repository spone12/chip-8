using Raylib_cs;

public class Display
{
    public const ushort ScreenWidth = 64;            // Ширина дисплея CHIP-8
    public const ushort ScreenHeight = 32;           // Высота дисплея CHIP-8
    public const ushort PixelSize = 10;           // Размер пикселей
    public const ushort FPS = 60;

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

    public bool SetPixels(ushort opcode, byte[] V, ushort I, Memory memory) {
        
        byte height = (byte)(opcode & 0x000F);
        byte x = (byte)((opcode & 0x0F00) >> 8);
        byte y = (byte)((opcode & 0x00F0) >> 4);
        
        bool collision = false;
            
        for (int row = 0; row < height; row++) {
            byte spriteByte = memory[I + row];
            
            for (byte bit = 0; bit < 8; bit++) {
                int px = (V[x] + bit) % ScreenWidth;
                int py = (V[y] + row) % ScreenHeight;
                bool spritePixel = ((spriteByte >> (7 - bit)) & 1) == 1;

                if (spritePixel) {
                    if (pixels[px, py]) collision = true;
                    pixels[px, py] ^= true;
                }
            }
        }

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
