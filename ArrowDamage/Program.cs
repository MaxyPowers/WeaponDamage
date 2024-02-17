using System.Diagnostics;
using System.Runtime;

/*
 * 
 */

class WeaponDamage
{
    /// <summary>
    /// Резервное поле хранит результат броска d6
    /// </summary>
    private int roll;
    /// <summary>
    /// Сохраняет и возвращает результат броска d6.
    /// </summary>
    public int Roll
    {
        get { return roll; }
        set { roll = value; CalculateDamage(); }
    }

    /// <summary>
    /// Резервное поле "флаг" хранит значение True/False (Магическое/Не магическое)
    /// </summary>
    private bool magic;
    /// <summary>
    /// Свойство устанавливает значение True/False (Магическое/Не магическое)
    /// </summary>
    public bool Magic
    {
        get { return magic; }
        set { magic = value; CalculateDamage(); }
    }

    /// <summary>
    /// Резервное поле "флаг" хранит значение True/False (Огненное/Не огненное)
    /// </summary>
    private bool flaming;
    /// <summary>
    /// Свойство устанавливает значение True/False (Огненное/Не огненное)
    /// </summary>
    public bool Flaming
    {
        get { return flaming; }
        set { flaming = value; CalculateDamage(); }
    }

    /// <summary>
    /// Свойтво хранит общий урон от оружия
    /// </summary>
    public int Damage { get; protected set; }

    /// <summary>
    /// Конструктор имитирует бросок указанного количества шестигранных игровых костей.
    /// Конструктов вызывает метод CalculateDamage для расчета урона от оружия
    /// </summary>
    /// <param name="numberOfRoll">Количество шестигранных игровых костей</param>
    public WeaponDamage(int numberOfRoll)
    {
        if (numberOfRoll<=0) return;           //Если количество игральных костей меньше либо равно 0, выйти
        Random rnd = new Random();
        int preRoll = 0;                       //Хранит текущий результат броска шестигранной(ых) кости(ей)
        for (int i = 0; i < numberOfRoll; i++) //Цикл отработает заданное количество игральных костей
            preRoll += rnd.Next(1, 7);         //Имитировать бросок шестигранной кости и добавить к текущему результату

        roll = preRoll;                        //Сохранить результат броска в свойство
        CalculateDamage();
    }

    /// <summary>
    /// Метод для расчета урона оружия
    /// </summary>
    protected virtual void CalculateDamage() { /*Реализация метода в субклассах*/ }
}

class SwordDamage : WeaponDamage
{
    private const int BASE_DAMAGE = 3;  //Базовый урон для меча
    private const int FLAME_DAMAGE = 2; //Дополнительный урон для огненного меча

    /// <summary>
    /// Конструктор передаёт параметр количества шестигранных игральных костей конструктору базового класса
    /// </summary>
    /// <param name="startingRoll">Количество шестигранных игральных костей<</param>
    public SwordDamage(int startingRoll) : base(startingRoll) { }

    /// <summary>
    /// Вычисляет урон от меча в зависимости от текущих значений свойств.
    /// </summary>
    protected override void CalculateDamage()
    {
        decimal magicMultiplier = 1M;                         //Стандартное значения для расчета урона
        if (Magic) magicMultiplier = 1.75M;                   //Изменить значиние для магического меча
        Damage = BASE_DAMAGE;                                 //Урон равен базовому урону меча
        Damage = (int)(Roll * magicMultiplier) + BASE_DAMAGE; //Урон равен броску 3d6 * на маги. урон плюс базовый урон
        if (Flaming) Damage += FLAME_DAMAGE;                  //Если мечь огненый уроне увеличивается на доп. урон 
    }
}


class ArrowDamage : WeaponDamage
{
    private const decimal BASE_MULTIPLIER = 0.35M; //Множитель для базового урон стрелы
    private const decimal MAGIC_MULTIPLIER = 2.5M; //Множитель для урона магической стрелы
    private const decimal FLAME_DAMAGE = 1.25M;    //Дополнительный урон для огненной стрелы

    /// <summary>
    /// Конструктор передаёт параметр количества шестигранных игральных костей конструктору базового класса
    /// </summary>
    /// <param name="startingRoll">Количество шестигранных игральных костей<</param>
    public ArrowDamage(int startingRoll) : base(startingRoll) { }
    
    /// <summary>
    /// Вычисляет повреждения в зависимости от текущих значений свойств.
    /// </summary>
    protected override void CalculateDamage()
    {
        decimal baseDamage = Roll * BASE_MULTIPLIER;                        //Базовый урон получается при умножении
                                                                            //броска на базовый множитель
        if (Magic) baseDamage *= MAGIC_MULTIPLIER;                          //Если стерла магическая, умножить на 
                                                                            //магический множитель
        if (Flaming) Damage = (int)Math.Ceiling(baseDamage + FLAME_DAMAGE); //Если мечь огненный, добавить доп. урон
        else Damage = (int)Math.Ceiling(baseDamage);                        //округлить и привести к целому числу
    }
}

class Program
{
    static void Main(string[] args)
    {
        SwordDamage swordDamage = new SwordDamage(3);
        ArrowDamage arrowDamage = new ArrowDamage(1);
        while (true)
        {
            Console.Clear();                                                          //Очистить консоль
            Console.Write("0 for no magic/flaming, 1 for magic, 2 for flaming, " +
                                  "3 for both, anything else to quit: ");
                    char key = Console.ReadKey().KeyChar;                             //Перехватить знак с консоли
                    if (key != '0' && key != '1' && key != '2' && key != '3') return; //Выйти если знак не соответствует

            Console.Write("\nS for sword, A for arrow, anything else to quit: ");
            char weaponKey = Char.ToUpper(Console.ReadKey().KeyChar);                 //Перехватить знак

            switch (weaponKey)
            {
                case 'S': //Выбран меч
                    
                    swordDamage.Roll = RollDice(3); //Имитировать бросок 3d6, сохранить результат в свойство Roll
                    swordDamage.Magic = (key == '1' || key == '3');//Установить флаг True/False для магического свойства
                    swordDamage.Flaming = (key == '2' || key == '3');//Установить флаг True/False для огненного свойства
                    Console.WriteLine($"\nRolled {swordDamage.Roll} for {swordDamage.Damage} HP\n");//Выввести на консоль
                                                                                                    //результат броска и урон
                    break;

                case 'A': //Выбранна стрела
                    arrowDamage.Roll = RollDice(1);//Имитировать бросок 1d6, сохранить результат в свойство Roll
                    arrowDamage.Magic = (key == '1' || key == '3');//Установить флаг True/False для магического свойства
                    arrowDamage.Flaming = (key == '2' || key == '3');//Установить флаг True/False для огненного свойства
                    Console.WriteLine($"\nRolled {arrowDamage.Roll} for {arrowDamage.Damage} HP\n");//Выввести на консоль
                                                                                                    //результат броска и урон
                    break;
            }

            _ = Console.ReadKey(); //Ожидать нажатия клавиши
        }
    }

    /// <summary>
    /// Имитировать бросок заданного количества шестигранных игральных костей
    /// </summary>
    /// <param name="numberOfRolls">Количество шестигранных игральных костей</param>
    /// <returns></returns>
    private static int RollDice(int numberOfRolls)
    {
        if (numberOfRolls <= 0) return 0; //Если количество игральных костей ноль и меньше, вернуть ноль
                                          //нарушить работу программы, призвать силы зла, свергнуть диктаторов,
                                          //повергнуть мир в хаос и анархию, придумать другой способ проверки на ноль

        Random rand = new Random();
        int preRoll = 0;                        //Хранит результат броска d6
        for (int i = 0; i < numberOfRolls; i++) //Цикл отработает заданное количество бросков
            preRoll += rand.Next(1, 7);         //Сохранить сложить результат каждого броска
        return preRoll;                         //Вернуть результат броска
    }
}