

public class CPU
{
    private byte[] V = new byte[16];       // 16 регистров общего назначения V0–VF
    private ushort I;                      // Специальный регистр I — хранит адрес в памяти
    private ushort pc = 0x200;             // Программный счётчик. Начинается с адреса 0x200
    private Stack<ushort> stack = new Stack<ushort>(16); // Стек вызовов
    
    private byte delayTimer = 0;           // Таймер задержки
    private byte soundTimer = 0;           // Таймер звука
    
    private Memory memory;                 // Ссылка на память
    private Display display;               // Ссылка на дисплей
    private Keyboard keyboard;             // Ссылка на клавиатуру

    public bool drawFlag = false;         // Отрисовать кадр
    public bool isDrawFlagRender = false; // Отрисовка кадров только через опкод Dxyn
    
    private bool waitingForKey = false;   // Ожидание ввода с клавиатуры
    private byte waitingRegister = 0;     // Ожидаемый регистр ввода
    
    public CPU(Memory memory, Display display, Keyboard keyboard)
    {
        this.memory = memory;             // Присвоение памяти
        this.display = display;           // Присвоение дисплея
        this.keyboard = keyboard;         // Присвоение клавиатуры
    }

    public void Cycle()
    {
        // Fx0A - Ожидание ввода с клавиатуры
        if (waitingForKey) {
            if (keyboard.WaitForKey(V, waitingRegister)) {
                waitingForKey = false;
                pc += 2;
            }
        }
        
        // Считывание двух байтов из памяти (один опкод = 2 байта)
        ushort opcode = (ushort)((memory[pc] << 8) | memory[pc + 1]);
        Execute(opcode); // Выполние опкода

        // Уменьшение таймера задержки
        if (delayTimer > 0) {
            delayTimer--;
        }
        
        // Уменьшение таймера звука
        if (soundTimer > 0) {
            soundTimer--;
        }
    }

    private void Execute(ushort opcode)
    {
        // 00E0 CLS Очистить экран
        if (opcode == 0x00E0) {
            display.Clear();
            return;
        }
        
        // 00EE RET Возвратиться из подпрограммы
        if (opcode == 0x00EE) {
            if (stack.Count == 0) return;
            pc = stack.Pop();
            return;
        }

        ushort NNN = (ushort)(opcode & 0x0FFF);
        byte NN = (byte)(opcode & 0x00FF);
        byte x = (byte)((opcode & 0x0F00) >> 8);
        byte y = (byte)((opcode & 0x00F0) >> 4);
        
        // 1NNN — "Прыжок на адрес NNN"
        if ((opcode & 0xF000) == 0x1000) {
            pc = NNN;
            return;
        }
        
        // 2NNN CALL - Вызов подпрограммы по адресу NNN
        if ((opcode & 0xF000) == 0x2000) {
            
            ushort address = NNN;
            
            stack.Push((ushort)(pc + 2));
            pc = address;
            return;
        }
        
        // 3XNN SE Vx, kk - проверяет, равно ли значение регистра VX (где X — это номер регистра от 0 до 15) константе NN.
        if ((opcode & 0xF000) == 0x3000) {
            
            if (V[x] == NN) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // 4XNN SNE Vx, kk - Пропустить следующую инструкцию, если значение регистра VX НЕ РАВНО NN
        if ((opcode & 0xF000) == 0x4000) {

            if (V[x] != NN) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // 5XY0 SE Vx, Vy - Пропустить следующую инструкцию, если V[X] == V[Y]
        // 0xF00F маскирует только X, Y и самый последний ниббл
        if ((opcode & 0xF00F) == 0x5000) {
            
            if (V[x] == V[y]) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // 6xNN  LD Vx, NN  Загрузить в регистр Vx число NN, т.е. Vx = NN
        if ((opcode & 0xF000) == 0x6000) {
            
            V[x] = NN;
            pc += 2;
            return;
        }
        
        // 7xNN ADD Vx, NN Установить Vx = Vx + NN
        if ((opcode & 0xF000) == 0x7000) {
            
            V[x] = (byte)(V[x] + NN);
            pc += 2;
            return;
        }
        
        // 8xy0 LD Vx, Vy Установить Vx = Vy
        if ((opcode & 0xF00F) == 0x8000) {
            
            V[x] = V[y];
            pc += 2;
            return;
        }
        
        // 8xy1 OR Vx, Vy Выполнить операцию дизъюнкция (логическое “ИЛИ”) над значениями регистров Vx и Vy, результат сохранить в Vx. Т.е. Vx = Vx | Vy
        if ((opcode & 0xF00F) == 0x8001) {
            
            V[x] = (byte)(V[x] | V[y]);
            pc += 2;
            return;
        }
        
        // 8xy2 AND Vx, Vy Выполнить операцию конъюнкция (логическое “И”) над значениями регистров Vx и Vy, результат сохранить в Vx. Т.е. Vx = Vx & Vy
        if ((opcode & 0xF00F) == 0x8002) {
            
            V[x] = (byte)(V[x] & V[y]);
            pc += 2;
            return;
        }
        
        // 8xy3 XOR Vx, Vy Выполнить операцию “исключающее ИЛИ” над значениями регистров Vx и Vy, результат сохранить в Vx. Т.е. Vx = Vx ^ Vy
        if ((opcode & 0xF00F) == 0x8003) {
            
            V[x] = (byte)(V[x] ^ V[y]);
            pc += 2;
            return;
        }
        
        // 8xy4 ADD Vx, Vy Значения Vx и Vy суммируются.
        // Если результат больше, чем 8 бит (т.е.> 255) VF устанавливается в 1, иначе 0.
        // Только младшие 8 бит результата сохраняются в Vx. Т.е. Vx = Vx + Vy
        if ((opcode & 0xF00F) == 0x8004) {

            ushort sumXY = (ushort)(V[x] + V[y]);
            
            if (sumXY > 0xFF) {
                V[0xF] = 1;
            } else {
                V[0xF] = 0;
            }

            V[x] = (byte)(sumXY & 0xFF);
            pc += 2;
            return;
        }
        
        // 8xy5 SUB Vx, Vy Если Vx >= Vy, то VF устанавливается в 1, иначе 0. Затем Vy вычитается из Vx, а результат сохраняется в Vx. Т.е. Vx = Vx - Vy
        if ((opcode & 0xF00F) == 0x8005) {

            if (V[x] >= V[y]) {
                V[0xF] = 1;
            } else {
                V[0xF] = 0;  
            }

            V[x] = (byte)(V[x] - V[y]);
            pc += 2;
            return;
        }
        
        // 8xy6 SHR Vx {, Vy} Операция сдвига вправо на 1 бит.
        // Сдвигается регистр Vx. Т.е. Vx = Vx » 1.
        // До операции сдвига выполняется следующее: если младший бит (самый правый) регистра Vx равен 1, то VF = 1, иначе VF = 0
        if ((opcode & 0xF00F) == 0x8006) {

            if ((V[x] & 0x1) == 1) {
                V[0xF] = 1;
            } else {
                V[0xF] = 0;
            }
            
            V[x] = (byte)(V[x] >> 1);
            pc += 2;
            return;
        }
        
        // 8xy7 SUBN Vx, Vy Если Vy >= Vx, то VF устанавливается в 1, иначе 0.
        // Тогда Vx вычитается из Vy, и результат сохраняется в Vx. Т.е. Vx = Vy - Vx
        if ((opcode & 0xF00F) == 0x8007) {

            if (V[y] >= V[x]) {
                V[0xF] = 1;
            } else {
                V[0xF] = 0;  
            }

            V[x] = (byte)(V[y] - V[x]);
            pc += 2;
            return;
        }
        
        // 8xyE SHL Vx {, Vy} Операция сдвига влево на 1 бит.
        // Сдвигается регистр Vx. Т.е. Vx = Vx « 1.
        // До операции сдвига выполняется следующее: если младший бит (самый правый) регистра Vx равен 1, то VF = 1, иначе VF = 0
        if ((opcode & 0xF00E) == 0x800E) {

            // Проверка старшего (левого) бита: установлен ли он?
            V[0xF] = (byte)((V[x] & 0x80) != 0 ? 1 : 0);
            
            V[x] = (byte)(V[x] << 1);
            pc += 2;
            return;
        }

        // 9xy0 SNE Vx, Vy Пропустить следующую инструкцию, если Vx != Vy
        if ((opcode & 0xF000) == 0x9000) {

            if (V[x] != V[y]) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // Annn LD I, NNN Значение регистра I устанавливается в NNN
        if ((opcode & 0xF000) == 0xA000) {
            
            I = NNN;
            pc += 2;
            return;
        }
        
        // Bnnn JP V0, NNN Перейти по адресу NNN + значение в регистре V0.
        if ((opcode & 0xF000) == 0xB000) {
            
            pc = (ushort)(NNN + V[0]);
            return;
        }
        
        //CxNN RND Vx, NN Устанавливается Vx = (случайное число от 0 до 255) & NN
        if ((opcode & 0xF000) == 0xC000) {
            
            Random rand = new Random();
            int randByte = rand.Next(0, 256);

            V[x] = (byte)(randByte & NN);
            pc += 2;
            return;
        }
        
        // Dxyn DRW Vx, Vy, n Нарисовать на экране спрайт.
        // Эта инструкция считывает n байт по адресу содержащемуся в регистре I и рисует их на экране в виде спрайта c координатой Vx, Vy.
        // Спрайты рисуются на экран по методу операции XOR, то есть если в том месте где мы рисуем спрайт уже есть нарисованные пиксели - они стираются, если их нет - рисуются.
        // Если хоть один пиксель был стерт, то VF устанавливается в 1, иначе в 0.
        if ((opcode & 0xF000) == 0xD000) {
            
            bool collision = display.SetPixels(opcode, V, I, memory);
            V[0xF] = (byte)(collision ? 1 : 0);
            drawFlag = true;
            pc += 2;
            return;
        }
        
        // Ex9E SKP Vx Пропустить следующую команду если клавиша, номер которой хранится в регистре Vx, нажата
        if ((opcode & 0xF0FF) == 0xE09E) {
            
            if (keyboard.IsKeyDown(V[x])) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // ExA1 SKNP Vx Пропустить следующую команду если клавиша, номер которой хранится в регистре Vx, не нажата
        if ((opcode & 0xF0FF) == 0xE0A1) {
            
            if (!keyboard.IsKeyDown(V[x])) {
                pc += 4;
            } else {
                pc += 2;
            }
            return;
        }
        
        // Fx07 LD Vx, DT Скопировать значение таймера задержки в регистр Vx
        if ((opcode & 0xFF07) == 0xF007) {

            V[x] = delayTimer;
            pc += 2;
            return;
        }
        
        // Fx0A LD Vx, K Ждать нажатия любой клавиши.
        // Как только клавиша будет нажата записать ее номер в регистр Vx и перейти к выполнению следующей инструкции.
        if ((opcode & 0xF0FF) == 0xF00A) {
            
            waitingForKey = true;
            waitingRegister = x;
            return;
        } 
        
        // Fx15 LD DT, Vx Установить значение таймера задержки равным значению регистра Vx
        if ((opcode & 0xF0FF) == 0xF015) {

            delayTimer = V[x];
            pc += 2;
            return;
        }
        
        // Fx18 LD ST, Vx Установить значение звукового таймера равным значению регистра Vx
        if ((opcode & 0xF0FF) == 0xF018) {
            soundTimer = V[x];
            pc += 2;
            return;
        }

        // Fx1E ADD I, Vx Сложить значения регистров I и Vx, результат сохранить в I. Т.е. I = I + Vx
        if ((opcode & 0xF0FF) == 0xF01E) {
            I = (ushort)(I + V[x]);
            pc += 2;
            return;
        }

        // Fx29 LD F, Vx Используется для вывода на экран символов встроенного шрифта размером 4x5 пикселей.
        // Команда загружает в регистр I адрес спрайта, значение которого находится в Vx.
        // Например, нам надо вывести на экран цифру 5. Для этого загружаем в Vx число 5.
        // Потом команда LD F, Vx загрузит адрес спрайта, содержащего цифру 5, в регистр I
        if ((opcode & 0xF0FF) == 0xF029) {
            I = (ushort)(V[x] * 5);
            pc += 2;
            return;
        }

        // Fx33 LD B, Vx Сохранить значение регистра Vx в двоично-десятичном (BCD) представлении по адресам I, I+1 и I+2
        if ((opcode & 0xF0FF) == 0xF033) {

            byte value = V[x];
            
            memory[I] = (byte)(value / 100); // Сотни
            memory[I + 1] = (byte)((value / 10) % 10); // Десятки
            memory[I + 2] = (byte)(value % 10); // Единицы
            
            pc += 2;
            return;
        }

       // Fx55 LD [I], Vx Сохранить значения регистров от V0 до Vx в памяти, начиная с адреса находящегося в I
       if ((opcode & 0xF0FF) == 0xF055) {
        
           for (byte i = 0; i <= x; i++) {
               memory[I + i] = V[i];
           }
           pc += 2;
           return;
       }

       // Fx65 LD Vx, [I] Загрузить значения регистров от V0 до Vx из памяти, начиная с адреса находящегося в I
       if ((opcode & 0xF0FF) == 0xF065) {

           for (byte i = 0; i <= x; i++) {
               V[i] = memory[I + i];
           }
           pc += 2;
           return;
       }
       
       Console.WriteLine($"Неизвестный opcode: {opcode:X4}");
    }
}
