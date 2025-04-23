

public class Display
{
    public const int Width = 64;            // Ширина дисплея CHIP-8
    public const int Height = 32;           // Высота дисплея CHIP-8
    private bool[,] pixels = new bool[Width, Height]; // Массив для хранения состояния пикселей

    public void Clear()
    {
        Array.Clear(pixels, 0, pixels.Length); // Очистка экрана (все пиксели в false)
    }

    public bool Draw(ushort opcode, byte[] V, ushort I, Memory memory) {
        
        byte height = (byte)(opcode & 0x000F);
        byte x = (byte)((opcode & 0x0F00) >> 8);
        byte y = (byte)((opcode & 0x00F0) >> 4);
        
        bool collision = false;
            
        for (int row = 0; row < height; row++) {
            byte spriteByte = memory[I + row];
            
            for (byte bit = 0; bit < 8; bit++) {
                int px = (V[x] + bit) % Width;
                int py = (V[y] + row) % Height;
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
        for (ushort y = 0; y < Height; y++) {
            string line = "";
            
            for (ushort x = 0; x < Width; x++) {
                line += pixels[x, y] ? "█" : " ";
            }
            Console.WriteLine(line);
            Thread.Sleep(5);
        }
    }
}
