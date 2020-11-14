using System;
using Menu;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Menu
{
    public class Reg_and_auto
    {
        static int q = 5;
        static int a = 0;
        static int b = 0;
        static List<string> Logins = new List<string>(); // Логин.       
        static List<string> Passwords = new List<string>(); // Пароль.
        static public void User() // Класс, который хранит регистрационные данные.
        {
            FileStream daa = new FileStream("meow.txt", FileMode.OpenOrCreate);
            daa.Close();
            int count = File.ReadAllLines("meow.txt").Length;
            FileStream data = new FileStream("meow.txt", FileMode.Open);
            StreamReader all = new StreamReader(data);
            int i = 0;
            while (i < count)
            {
                Logins.Add(all.ReadLine());
                i++;
                Passwords.Add(all.ReadLine());
                i++;
            }
            all.Close();
            data.Dispose();
        }
        static public class Log
        {
            static public void Write()
            {
                string filename = "meow.txt";
                FileStream data = new FileStream(filename, FileMode.Open);
                StreamWriter log = new StreamWriter(data);
                data.Seek(0, SeekOrigin.End);
                Console.WriteLine("Введите логин:");
                string text = Console.ReadLine();
                prov_wr(text,data,log);
                log.WriteLine(text);
                log.Close();
                data.Dispose();
                Pass.Write();
            }
            static private void prov_wr(string text,FileStream data,StreamWriter log)
            {
                if (Logins.Contains(text))
                {

                    Console.WriteLine("Такой логин уже есть!");
                    log.Close();
                    data.Dispose();
                    Write();
                }
                else return;

            }
            static private void prov_re(string text)
            {

		User();
                if (Logins.Contains(text))
                {
                    a = Logins.IndexOf(text);
                    return;
                }
                else {

                    Console.WriteLine($"Неправильно введен логин! У вас есть {q} попыток!");
                    q--;
                    if (q == 0)
                    { reg_or_auto(); }
                    Reade(); 
                    
                }
         
            }
            static public void Reade()
            {

                Console.WriteLine("Введите логин");
                string text = Console.ReadLine();
                prov_re(text);
                q = 5;
                Pass.Reade();
            }
        }
        static class Pass
        {
            static public void Write()
            {
                string filename = "meow.txt";
                FileStream data = new FileStream(filename, FileMode.OpenOrCreate);
                StreamWriter pasw = new StreamWriter(data);
                data.Seek(0, SeekOrigin.End);
                Console.WriteLine("Введите пароль:");
                string text = Console.ReadLine();
                pasw.WriteLine(text);
                pasw.Close();
                data.Dispose();
                Log.Reade();
                //переход в основное меню
            }
            static private void prov(string text)
            {
                if (Passwords.Contains(text))
                {
                    b = Passwords.IndexOf(text);
                }
                if (b == a)
                { return; }
                else { q--; Console.WriteLine($"Неправильно введен пароль! У вас есть {q-1} попыток!"); 
                    if (q == 0)
                    { reg_or_auto(); }
                    Reade();
                    
                }
               

            }
            static public void Reade()
            {
                Console.WriteLine("Введите пароль: ");
                string text = Console.ReadLine();
                prov(text);

                Menus.Meny();

                //переход в основное меню
            }
        }
        static public void reg_or_auto()
        {
            q = 5;
            Menu_reg.Meny();
        }
    }
    public class Menu_reg
    {
        delegate void method();
        static public void Meny()
        {
            Reg_and_auto.User();
            string[] items = { "Регистрация", "Авторизация", "Выход" };
            method[] methods = new method[] { Method1, Method2, Exit };
            ConsoleMenu menu = new ConsoleMenu(items);
            int menuResult;
            do
            {
                menuResult = menu.PrintMenu();
                methods[menuResult]();
                Console.WriteLine("Для продолжения нажмите любую клавишу");
                Console.ReadKey();
            } while (menuResult != items.Length - 1);
        }
        static void Method1()
        {
            Console.Beep(440, 300);
            Reg_and_auto.Log.Write();


        }
        static void Method2()
        {
            Console.Beep(440, 300);
            Reg_and_auto.Log.Reade();
        }
        static void Exit()
        {
            Console.Beep(440, 300);
            Environment.Exit(0);

        }
        class ConsoleMenu
        {
            string[] menuItems;
            int counter = 0;
            public ConsoleMenu(string[] menuItems)
            {
                this.menuItems = menuItems;
            }

            public int PrintMenu()
            {
                ConsoleKeyInfo key;
                do
                {
                    Console.Clear();
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        if (counter == i)
                        {
                            Console.BackgroundColor = ConsoleColor.Cyan;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine(menuItems[i]);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                            Console.WriteLine(menuItems[i]);

                    }
                    key = Console.ReadKey();
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        counter--;
                        if (counter == -1) counter = menuItems.Length - 1;
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        counter++;
                        if (counter == menuItems.Length) counter = 0;
                    }
                }
                while (key.Key != ConsoleKey.Enter);
                return counter;
            }

        }
    }
    public class Menus
    {

        public static int x = 0;
        private static string[,] polygon = new string[4, 4];
        private static string[] tabs = new string[4] { "Snake ::", "Saper ::", "x/0 ::", "Exit ::" };
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

        public static void Meny()
        {

            Thread myThread = new Thread(new ThreadStart(Music));
            myThread.Start(); // запускаем поток
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
        private static void Music()
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
    class Program
    {
        static void Main()
        {
            Menu_reg.Meny();
        }
    }
