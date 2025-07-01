using Raylib_cs;

public class Keyboard
{
    /// <summary>
    /// 16 клавиш (0–F), как на старом калькуляторе
    /// </summary>
    private readonly KeyboardKey[] keyMap = new KeyboardKey[16] 
    {
        KeyboardKey.X,     // 0
        KeyboardKey.One,   // 1
        KeyboardKey.Two,   // 2
        KeyboardKey.Three, // 3
        KeyboardKey.Q,     // 4
        KeyboardKey.W,     // 5
        KeyboardKey.E,     // 6
        KeyboardKey.A,     // 7
        KeyboardKey.S,     // 8
        KeyboardKey.D,     // 9
        KeyboardKey.Z,     // A (10)
        KeyboardKey.C,     // B (11)
        KeyboardKey.Four,  // C (12)
        KeyboardKey.R,     // D (13)
        KeyboardKey.F,     // E (14)
        KeyboardKey.V      // F (15)
    };
    
    /// <summary>
    /// Проверка нажатия клавиши
    /// </summary>
    /// <param name="chip8Key"></param>
    /// <returns>
    /// Возвращает, была ли нажата клавиша
    /// </returns>
    public bool IsKeyDown(int chip8Key)
    {
        return Raylib.IsKeyDown(keyMap[chip8Key]);
    }
    
    /// <summary>
    /// Ожидание нажатия клавиши
    /// </summary>
    /// <param name="V"></param>
    /// <param name="waitingRegister"></param>
    /// <returns>
    /// Возврат bool'евого значения ожидания, что клавиша была нажата
    /// </returns>
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
}
