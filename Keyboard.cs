using Raylib_cs;

public class Keyboard
{
    private bool[] keys = new bool[16];     // 16 клавиш (0–F), как на старом калькуляторе

    public bool IsKeyPressed(int index) => keys[index]; // Проверить, нажата ли клавиша

    public void SetKeyDown(int index) => keys[index] = true; // Установить клавишу как нажатую
    public void SetKeyUp(int index) => keys[index] = false;  // Установить клавишу как отпущенную
    
    Dictionary<ConsoleKey, int> _keyMapConsole = new Dictionary<ConsoleKey, int>
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

    private Dictionary<int, KeyboardKey> _keyMap = new Dictionary<int, KeyboardKey>
    {
        { 0x0, KeyboardKey.X },
        { 0x1, KeyboardKey.One },
        { 0x2, KeyboardKey.Two },
        { 0x3, KeyboardKey.Three },
        { 0x4, KeyboardKey.Q },
        { 0x5, KeyboardKey.W },
        { 0x6, KeyboardKey.E },
        { 0x7, KeyboardKey.A },
        { 0x8, KeyboardKey.S },
        { 0x9, KeyboardKey.D },
        { 0xA, KeyboardKey.Z },
        { 0xB, KeyboardKey.C },
        { 0xC, KeyboardKey.Four },
        { 0xD, KeyboardKey.R },
        { 0xE, KeyboardKey.F },
        { 0xF, KeyboardKey.V },
    };

    public void SetKey(byte x, byte[] V) {
    
        while (true)
        {
            for (byte i = 0; i < 0xF; i++) {
                if (Raylib.IsKeyPressed(_keyMap[i])) {
                    V[x] = i;
                    keys[i] = true;
                    
                    return;
                } else {
                    keys[i] = false;
                }
            }
        }
    }

    public void SetKeyConsole(byte x, byte[] V) {
        
        int chip8Key = -1;
        
        while (true) {

            if (Console.KeyAvailable) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                if (_keyMapConsole.TryGetValue(keyInfo.Key, out chip8Key)) {
                    V[x] = (byte)chip8Key;
                    break;
                }
            }
            
            Thread.Sleep(10);
        }
    }
}

