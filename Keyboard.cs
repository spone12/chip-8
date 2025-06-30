using Raylib_cs;

public class Keyboard
{
    private bool[] keys = new bool[16];
    
    private readonly KeyboardKey[] keyMap = new KeyboardKey[16] // 16 клавиш (0–F), как на старом калькуляторе
    {
        KeyboardKey.X, KeyboardKey.One, KeyboardKey.Two, KeyboardKey.Three,
        KeyboardKey.Q, KeyboardKey.W, KeyboardKey.E, KeyboardKey.A,
        KeyboardKey.S, KeyboardKey.D, KeyboardKey.Z, KeyboardKey.C,
        KeyboardKey.Four, KeyboardKey.R, KeyboardKey.F, KeyboardKey.V
    };
    
    public bool IsKeyDown(int chip8Key)
    {
        return Raylib.IsKeyDown(keyMap[chip8Key]);
    }

    public bool WaitForKey(byte[] V, byte waitingRegister) 
    {
        for (byte i = 0; i <= 0xF; i++) {
            if (Raylib.IsKeyPressed(keyMap[i])) {
                V[waitingRegister] = i;
                return true;
            }
        }

        return false;
    }
    
    public void Update()
    {
        for (byte i = 0; i <= 0xF; i++) {
            keys[i] = Raylib.IsKeyDown(keyMap[i]);
        }
    }
}
