using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;


namespace Menu
{
   
    public class Users // Класс, который хранит регистрационные данные.
    {
        public List<string> Logins = new List<string>(); // Логин.
        public List<string> Passwords = new List<string>(); // Пароль.
    }
    public class Reg_and_auto
    {
        static public void reg()
        {
            string filename = "meow.txt";
            FileStream data = new FileStream(filename, FileMode.OpenOrCreate);
            StreamWriter log = new StreamWriter(data);
            data.Seek(0, SeekOrigin.End);
            Console.WriteLine("Введите логин");
            string text = Console.ReadLine();
            log.WriteLine(text);
            log.Flush();
            StreamWriter pasw = new StreamWriter(data);
            data.Seek(0, SeekOrigin.End);
            Console.WriteLine("Введите пароль");
            text = Console.ReadLine();
            pasw.WriteLine(text);
            pasw.Close();
        }
        void auto()
        {

        }
    }


    class Menu
    {
      
        
        public static int x = 0;
        public static string[,] polygon = new string[4, 4];
        public static string[] tabs = new string[4] { "Snake ::", "choto ::", "x/0 ::", "Exit ::" };
        private static bool[] setts = new bool[4] { false, false, false, false }; //0 - snake; 1 - choto, 2, - x/o
        public static void Render()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (polygon[i, j] == tabs[i])
                    {
                        bool active = setts[i];
                        Console.Write(polygon[i, j]);
                        if (active)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" ON");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" OFF");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write(polygon[i, j] + " ");
                    }
                }
                Console.WriteLine();
            }
        }
        public static void Update(int dir, bool init)
        {
            if (!init)
            {
                int[] delta_x = new int[2] { -1, 1 };
                int _X = delta_x[dir] + x;
                if (_X > 3 || _X < 0)
                    return;
                polygon[x, 0] = "";
                x = _X;
                polygon[_X, 0] = "-->";
            }
            else
            {
                polygon[0, 1] = tabs[0];
                polygon[1, 1] = tabs[1];
                polygon[2, 1] = tabs[2];
                polygon[3, 1] = tabs[3];
            }
            Console.Clear();
            Render();
            return;
        }
        public static void InitHacks()
        {
            while (true)
            {
                if (setts[0])
                {
                    Console.Beep(440, 300);
                   //Snake
                   
                }
                else if (setts[1])
                {
                    Console.Beep(440, 300);
                   
                    //2 Game
                }
                else if (setts[2])
                {
                    Console.Beep(440, 300);
                 //3 Game

                }
               
                else if (setts[3])
                {
                    Console.Beep(440, 300);
                    if (true)
                    {
                        Environment.Exit(0);
                    }
                    //make Exit
                }


            }
        }
        static void Main(string[] args)
        {

            Thread myThread = new Thread(new ThreadStart(Music));
            myThread.Start(); // запускаем поток
            Reg_and_auto.reg();
            Console.CursorVisible = false;
            Console.SetWindowPosition(0, 0);
            Console.Title = "Menu";
            polygon[0, 0] = "-->";
            Thread th = new Thread(InitHacks);
            Update(0, true);
            th.Start();
            while (true)
            {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow)
                {
                    Update(0, false);
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    Update(1, false);
                }
                else if (key == ConsoleKey.Enter)
                {
                    setts[x] = !setts[x];
                    Update(2, true);

                }
            }
        }
        public static void Music()
        {        
            for (int i = 1; i < 9; i++)
            {
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(932, 150);
                Thread.Sleep(150);
                Console.Beep(1047, 150);
                Thread.Sleep(150);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(699, 150);
                Thread.Sleep(150);
                Console.Beep(740, 150);
                Thread.Sleep(150);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(932, 150);
                Thread.Sleep(150);
                Console.Beep(1047, 150);
                Thread.Sleep(150);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(784, 150);
                Thread.Sleep(300);
                Console.Beep(699, 150);
                Thread.Sleep(150);
                Console.Beep(740, 150);
                Thread.Sleep(150);
                Console.Beep(932, 150);
                Console.Beep(784, 150);
                Console.Beep(587, 1200);
                Thread.Sleep(75);
                Console.Beep(932, 150);
                Console.Beep(784, 150);
                Console.Beep(554, 1200);
                Thread.Sleep(75);
                Console.Beep(932, 150);
                Console.Beep(784, 150);
                Console.Beep(523, 1200);
                Thread.Sleep(150);
                Console.Beep(466, 150);
                Console.Beep(523, 150);
            }
        }
    }
}
