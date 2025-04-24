

public class Keyboard
{
    private bool[] keys = new bool[16];     // 16 клавиш (0–F), как на старом калькуляторе

    public bool IsKeyPressed(int index) => keys[index]; // Проверить, нажата ли клавиша

    public void SetKeyDown(int index) => keys[index] = true; // Установить клавишу как нажатую
    public void SetKeyUp(int index) => keys[index] = false;  // Установить клавишу как отпущенную
    
    Dictionary<ConsoleKey, int> _keyMap = new Dictionary<ConsoleKey, int>
    {
        { ConsoleKey.D1, 0x1 },
        { ConsoleKey.D2, 0x2 },
        { ConsoleKey.D3, 0x3 },
        { ConsoleKey.D4, 0xC },
        { ConsoleKey.Q,   0x4 },
        { ConsoleKey.W,   0x5 },
        { ConsoleKey.E,   0x6 },
        { ConsoleKey.R,   0xD },
        { ConsoleKey.A,   0x7 },
        { ConsoleKey.S,   0x8 },
        { ConsoleKey.D,   0x9 },
        { ConsoleKey.F,   0xE },
        { ConsoleKey.Z,   0xA },
        { ConsoleKey.X,   0x0 },
        { ConsoleKey.C,   0xB },
        { ConsoleKey.V,   0xF }
    };

    public void SetKey(byte x, byte[] V) {
        
        int chip8Key = -1;
        
        while (true) {

            if (Console.KeyAvailable) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                if (_keyMap.TryGetValue(keyInfo.Key, out chip8Key)) {
                    V[x] = (byte)chip8Key;
                    break;
                }
            }
            
            Thread.Sleep(10);
        }
    }
}

