using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using static System.Console;
using System.IO;
using System.Text;
using System.Threading;

namespace SimpleProject
{
    class Program
    {
        public static string Path = "Users.xml";
        


        public class ConsoleHelper
        {
        public static int MultipleChoice(params string[] options)
        {
            int optionsPerLine = options.Length;


            int currentSelection = 0;

            ConsoleKey key;

            CursorVisible = false;

            do
            {
                Clear();
                    int maxlen = 0;

                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i].Length > maxlen) maxlen = options[i].Length;
                    }

                    for (int i = 0; i < options.Length; i++)
                    {
                        if (i == currentSelection)
                        {
                            ForegroundColor = ConsoleColor.Red;
                            
                        }
                        for (int j = 0; j < maxlen + 4; j++)
                        {
                            Write("+");
                        }
                        SetCursorPosition(CursorLeft - 1, CursorTop + 1);
                        Write("+");
                        SetCursorPosition(0, CursorTop + 1);
                        for (int j = 0; j < maxlen + 4; j++)
                        {
                            Write("+");
                        }
                        SetCursorPosition(0, CursorTop - 1);
                        Write("+");
                        int a = (maxlen+4) - options[i].Length;
                        int b = a / 2;
                        SetCursorPosition(b-1, CursorTop);
                        Write($" {options[i]}");
                        SetCursorPosition(0, CursorTop + 2);
                        ResetColor();
                     }

                   key = ReadKey(true).Key;

                   switch (key)
                   {
                        case ConsoleKey.UpArrow:
                        {
                            if (currentSelection > 0)
                                currentSelection--;
                                Beep(100, 100);
                                break;
                        }
                        case ConsoleKey.DownArrow:
                        {
                            if (currentSelection < optionsPerLine - 1)
                                currentSelection++;
                                Beep(100, 100);
                                break;

                        }
                }
            } while (key != ConsoleKey.Enter);
                Beep(300, 100);
                CursorVisible = true;

            return currentSelection;
        }
    }

        public class User
        {
            public string Login { get; set; }
            delegate int LengthLogin(string s);
            delegate bool BoolPassword(string s1, string s2);
            public User() { }

            public bool Enter(int attempts)
            {
                Clear();
                if(attempts == 2) { ForegroundColor = ConsoleColor.Red; WriteLine("Осталось 2 попытки"); ResetColor(); }
                if (attempts == 1) { ForegroundColor = ConsoleColor.Red; WriteLine("Осталось 1 попытка"); ResetColor(); }
                var fileExists = File.Exists(Path);
                if(!fileExists) 
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("\t\t\tНет зарегестрированных пользователей! \n\t\t\tЧтобы войти сначала зарегистрируйтесь.");
                    ResetColor();
                    ReadLine();
                    return false;
                }

                string pass = "";
                Write("Введите логин: "); string name = ReadLine();
                XmlTextReader reader = null;
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(Path);
                    XmlElement xRoot = xDoc.DocumentElement;
                    foreach (XmlElement xnode in xRoot)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("Login");
                        bool i = false;
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name == "Login" && childnode.InnerText == name)
                            {
                                this.Login = childnode.InnerText;
                                i = true;
                            }

                            if (childnode.Name == "Password" && i == true)
                            {
                                pass = childnode.InnerText;
                                i = false;
                            }
                        }
                        
                    }

                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message);
                }

                Write("Введите пароль: ");
            List<char> passChar = new List<char>();
            while (true)
            {
                ConsoleKeyInfo cki = ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                    break;
                if(cki.Key == ConsoleKey.Backspace)
                {
                        if (passChar.Count > 0)
                        {
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            Write(" ");
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            passChar.RemoveAt(passChar.Count - 1);
                        }

                }
                else
                {
                    Write("*");
                    passChar.Add(cki.KeyChar);
                }
            }
            WriteLine();
            string passStr = null;
            foreach (char c in passChar)
                passStr += c;
                BoolPassword bp = (s1, s2) => s1 == s2;
                if (bp(pass, passStr))
                {
                    WriteLine("Правильный пароль");
                    ReadLine();
                }
                else
                {
                    if (attempts == 1)
                    {
                        ForegroundColor = ConsoleColor.Red;
                        int i = 3;
                        while (i != 0)
                        {
                            WriteLine($"Попыток больше нет. Твой комп сгорит через {i}");
                            i--;
                            Thread.Sleep(1000);
                        }
                        Environment.Exit(0);

                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("Неправильный логин или пароль! Попробуйте еще раз");
                        ResetColor();
                        ReadLine();
                        attempts--;
                        Enter(attempts);
                    }
                }

                return true;
            }
            public string Set_Login()
            {
                Clear();
                Write("Введите логин: ");
                string login = ReadLine();
                LengthLogin lengthLoginDelegate = s => s.Length;
                int lengthLogin = lengthLoginDelegate(login);
                if (lengthLogin > 10)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Слишком длинное имя. Максимум 10 символов!\n");
                    ForegroundColor = ConsoleColor.White;
                    ReadLine();
                    Set_Login();
                }
                return login;

            }
            public string Set_Password()
            {
                Write("Введите пароль: ");
                List<char> passChar1 = new List<char>();
                List<char> passChar2 = new List<char>();
                while (true)
                {
                    ConsoleKeyInfo cki = ReadKey(true);
                    if (cki.Key == ConsoleKey.Enter)
                        break;
                    if (cki.Key == ConsoleKey.Backspace)
                    {
                        if (passChar1.Count > 0)
                        {
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            Write(" ");
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            passChar1.RemoveAt(passChar1.Count - 1);
                        }
                    }
                    else
                    {
                        Write("*");
                        passChar1.Add(cki.KeyChar);
                    }
                }
                string password1 = null;
                foreach (char c in passChar1)
                    password1 += c;
                Write("\nПовторите пароль: ");
                while (true)
                {
                    ConsoleKeyInfo cki = ReadKey(true);
                    if (cki.Key == ConsoleKey.Enter)
                        break;
                    if (cki.Key == ConsoleKey.Backspace)
                    {
                        if (passChar2.Count > 0)
                        {
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            Write(" ");
                            SetCursorPosition(CursorLeft - 1, CursorTop);
                            passChar2.RemoveAt(passChar2.Count - 1);
                        }

                    }
                    else
                    {
                        Write("*");
                        passChar2.Add(cki.KeyChar);
                    }
                }
                string password2 = null;
                foreach (char c in passChar2)
                    password2 += c;

                BoolPassword bp = (s1, s2) => s1 == s2;
                if(bp(password1,password2))
                {
                    return password1;
                }
                else
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("\nПароли не совпадают!\n Попробуйте еще раз.");
                    ResetColor();
                    Set_Password();
                }
                return "fail";
            }
            public void Registration()
            {
                string login = Set_Login();
                string password = Set_Password();

                var fileExists = File.Exists(Path);

                if (fileExists)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Path);
                    XmlNode root = doc.DocumentElement;

                    XmlNode user = doc.CreateElement("User");
                    XmlNode elem1 = doc.CreateElement("Login");
                    XmlNode elem2 = doc.CreateElement("Password");
                    XmlNode elem3 = doc.CreateElement("Sapper");
                    XmlNode elem4 = doc.CreateElement("Snake");
                    XmlNode elem5 = doc.CreateElement("XO");

                    XmlNode text1 = doc.CreateTextNode(login);
                    XmlNode text2 = doc.CreateTextNode(password);
                    XmlNode text3 = doc.CreateTextNode("0");
                    XmlNode text4 = doc.CreateTextNode("0");
                    XmlNode text5 = doc.CreateTextNode("0");

                    elem1.AppendChild(text1);
                    elem3.AppendChild(text3);
                    elem2.AppendChild(text2);
                    elem4.AppendChild(text4);
                    elem5.AppendChild(text5);

                    user.AppendChild(elem1);
                    user.AppendChild(elem2);
                    user.AppendChild(elem3);
                    user.AppendChild(elem4);
                    user.AppendChild(elem5);

                    root.AppendChild(user);

                    doc.Save(Path);

                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("\nРегистрация прошла успешно!\n");
                    ResetColor();
                    ReadLine();

                }
                else
                {
                    XmlTextWriter writer = null;
                    try
                    {
                        writer = new XmlTextWriter(Path, System.Text.Encoding.Unicode);
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Users");
                        writer.WriteStartElement("User");
                        writer.WriteElementString("Login", login);
                        writer.WriteElementString("Password", password);
                        writer.WriteElementString("Sapper", "0");
                        writer.WriteElementString("Snake", "0");
                        writer.WriteElementString("XO", "0");
                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        ForegroundColor = ConsoleColor.Green;
                        WriteLine("Регистрация прошла успешно!\n");
                        ResetColor();
                        ReadLine();

                    }
                    catch (Exception ex)
                    {
                        WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (writer != null)
                            writer.Close();
                    }
                }



            }
        }
    
            static void Main()
            {
            WindowWidth = 100;
            WindowHeight = 20;
            User user = new User();
            bool enter = false;
            while (enter == false)
            {
                int select = ConsoleHelper.MultipleChoice("Вход", "Зарегистрироваться");

                switch (select)
                {
                    case 0: 
                        enter = user.Enter(3);
                        break;
                    case 1:
                        user.Registration();
                        break;

                }
            }
            

        }
    }
}

