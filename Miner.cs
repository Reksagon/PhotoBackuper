﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD = System.Diagnostics;

namespace Miner
{
    public class Miner
    {
        public int SizeMapHeight { get; set; }
        public int SizeMapWidth { get; set; }
        private Map mines;//'*' = -1 - mine, '^' = -2 - flag, -3 - empty cell (open), 0 - empty cell, 1,2,3,4,5,6,7,8 - numbers
        private Map map;
        private Statistic statistic;
        private int currentComplexity;
        public int AmountOfMines { get; set; }
        public User CurrentUser { get; protected set; }
        public Miner(User user)
        {
            SizeMapHeight = 9;
            SizeMapWidth = 9;
            AmountOfMines = 10;
            CurrentUser = user;
            statistic = new Statistic();
        }
        private void Initial()
        {
            Console.Clear();
            Console.WriteLine("\tНовая игра");
            currentComplexity = Menu.ChooseComplexity();
            switch (currentComplexity)
            {
                case 1:
                    SizeMapHeight = 9;
                    SizeMapWidth = 9;
                    AmountOfMines = 10;
                    break;
                case 2:
                    SizeMapHeight = 16;
                    SizeMapWidth = 16;
                    AmountOfMines = 40;
                    break;
                case 3:
                    SizeMapHeight = 16;
                    SizeMapWidth = 30;
                    AmountOfMines = 99;
                    break;
                case 4:
                    SizeMapHeight = Menu.EnterHeightMap();
                    SizeMapWidth = Menu.EnterWidthMap();
                    AmountOfMines = Menu.EnterAmountOfMines();
                    break;
            }
            mines = new Map(SizeMapHeight, SizeMapWidth, AmountOfMines);
            map = new Map(SizeMapHeight, SizeMapWidth, AmountOfMines);
        }
        public void StartArrows() //основной метод для игры через стрелки
        {
            bool endGame = false;
            bool bombed = false;
            bool firstMove = true;
            long startTime = 0;//логирование момента начала игры во времени
            double time;//затраченое время на игру
            int score;//заработаные очки
            bool exit = false;
            ConsoleKeyInfo key;
            Point cursor;
            Point newCursor;
            if (Menu.ChooseNewGameOrLiders() == 2) statistic.ShowLiders(Menu.ChooseComplexity());
            do
            {
                Initial();
                cursor = new Point();
                newCursor = new Point();
                endGame = false;
                bombed = false;
                firstMove = true;
                Show();
                ShowInfoCursor();
                map.SetCursor(newCursor);
                do
                {
                    key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter://поставить флаг
                            if (map[newCursor] != -2)//защита от повторной установки флага
                            {
                                map.CurrentMines--;
                                map[newCursor] = -2;
                                map.ReShowNumberMines();
                                map.WriteSymbol(newCursor);
                            }
                            break;
                        case ConsoleKey.Spacebar://открыть ячейку
                            if (firstMove)//если первый ход то растановка мин 
                            {
                                startTime = SD.Stopwatch.GetTimestamp();
                                mines.InitialBombs(newCursor);
                                firstMove = false;
                            }
                            if (map[newCursor] == -2)//если на открываемой ячейке стоит флаг возвращаем число мин
                            {
                                map.CurrentMines++;
                                map.ReShowNumberMines();
                            }
                            bombed = OpenCellCursor(newCursor);
                            break;
                        case ConsoleKey.LeftArrow://сдвиг указателя влево
                            cursor = newCursor.Clone();
                            newCursor.Letter--;
                            if (!map.IsInBorder(newCursor)) newCursor = cursor;
                            break;
                        case ConsoleKey.RightArrow://сдвиг указателя вправо
                            cursor = newCursor.Clone();
                            newCursor.Letter++;
                            if (!map.IsInBorder(newCursor)) newCursor = cursor;
                            break;
                        case ConsoleKey.UpArrow://сдвиг указателя вверх
                            cursor = newCursor.Clone();
                            newCursor.Number--;
                            if (!map.IsInBorder(newCursor)) newCursor = cursor;
                            break;
                        case ConsoleKey.DownArrow://сдвиг указателя вниз
                            cursor = newCursor.Clone();
                            newCursor.Number++;
                            if (!map.IsInBorder(newCursor)) newCursor = cursor;
                            break;
                        case ConsoleKey.Escape://выход
                            endGame = true;
                            Show();
                            break;
                        default:
                            map.WriteSymbol(newCursor);
                            break;
                    }
                    if (!endGame)
                    {
                        map.WriteSymbol(cursor);//повторная прорисовка зарисованой вводом ячейки
                        map.SetCursor(newCursor);//перемещение курсора на новую позицию
                    }
                    if (bombed)//вывод результата поражения
                    {
                        Show();
                        Console.WriteLine("Поражение!!!");
                        Console.WriteLine($"Затраченое время {GetTimeGame(startTime):0.00} с");
                        endGame = true;
                    }
                    else if (CheckWin(map))//проверка победы, вывод результата
                    {
                        time = GetTimeGame(startTime);
                        score = ScoreCount(time);
                        CurrentUser.Score += score;
                        statistic.Add(currentComplexity, CurrentUser.Name, score, time, DateTime.Now);
                        Show();
                        Console.WriteLine("Победа!!!");
                        Console.WriteLine($"Затраченое время {time:0.00} с");
                        Console.WriteLine($"Полученые очки {score}");
                        endGame = true;
                    }
                } while (!endGame);
                exit = Exit();
            } while (!exit);
        }
        public void StartCoordinate()//основной метод для игры через координаты
        {
            bool endGame = false;
            bool bombed = false;
            Point point = new Point();
            bool firstMove = true;
            long startTime = 0;//логирование момента начала игры во времени
            double time;//затраченое время на игру
            int score;//заработаные очки
            bool exit = false;
            if (Menu.ChooseNewGameOrLiders() == 2) statistic.ShowLiders(Menu.ChooseComplexity());
            do
            {
                Initial();
                endGame = false;
                bombed = false;
                firstMove = true;
                do
                {
                    Show();
                    point = Menu.EnterPoint(SizeMapHeight, SizeMapWidth);
                    if (Menu.ChooseActionOnCell() == 1)
                    {
                        if (firstMove)
                        {
                            startTime = SD.Stopwatch.GetTimestamp();
                             mines.InitialBombs(point);
                            firstMove = false;
                        }
                        if(map[point] == -2) map.CurrentMines++;
                        bombed = OpenCell(point);
                    }
                    else if (map[point] != -2)//защита от повторной установки флага
                    {
                        map[point] = -2;
                        map.CurrentMines--;
                    }
                    if (bombed)
                    {
                        Show();
                        Console.WriteLine("Поражение!!!");
                        Console.WriteLine($"Затраченое время {GetTimeGame(startTime):0.00} с");
                        endGame = true;
                    }
                    else if (CheckWin(map))
                    {
                        time = GetTimeGame(startTime);
                        score = ScoreCount(time);
                        CurrentUser.Score += score;
                        statistic.Add(currentComplexity, CurrentUser.Name, score, time, DateTime.Now);
                        Show();
                        Console.WriteLine("Победа!!!");
                        Console.WriteLine($"Затраченое время {time:0.00} с");
                        Console.WriteLine($"Полученые очки {score}");
                        endGame = true;
                    }
                } while (!endGame);
                exit = Exit();
            } while (!exit);
        }
        private void Show()//базовая отрисовка игрового поля
        {
            Console.Clear();
            CurrentUser.Show(SizeMapWidth, SizeMapHeight);
            map.Show();
        }
        private void ShowInfoCursor()
        {
            Console.WriteLine("Управление:\n" +
                "стрелки (вверх, вниз, влево, вправо) - движение по полю\n" +
                "cpace - открыть ячейку\n" +
                "enter - поставить флаг\n" +
                "esc - выход");
        }
        private double GetTimeGame(long startTime) => //время в "с" затраченое на игру
            (SD.Stopwatch.GetTimestamp() - startTime) / (double)SD.Stopwatch.Frequency;

        private bool OpenCell(Point point)//открытие ячеек,если бомба, тогда возврат true
        {
            map[point] = mines[point];
            if (mines[point] == -1)
            {
                return true;
            }
            else if (mines[point] == 0) OpenCellWithNull(point);
            return false;
        }
        private bool OpenCellCursor(Point point)//открытие ячеек,если бомба, тогда возврат true
        {
            map[point] = mines[point];
            map.WriteSymbol(point);
            if (mines[point] == -1)
            {
                return true;
            }
            else if (mines[point] == 0) OpenCellWithNullCursor(point);
            return false;
        }
        private void OpenCellWithNull(Point point)//открытие всех пустых смежных ячеек
        {
            if (map[point] == -2)
            {
                map.CurrentMines++;
            }
            if (mines[point] == 0)
            {
                map[point] = -3;
                mines[point] = -3;
                for (int i = point.Number - 1; i < point.Number + 2; i++)
                {
                    for (int j = point.Letter - 1; j < point.Letter + 2; j++)
                    {
                        if (mines.IsInBorder(new Point(i, j)) && mines[new Point(i, j)] != -3) 
                        {
                            OpenCellWithNull(new Point(i, j));
                        }
                    }
                }
            }
            map[point] = mines[point];
        }
        private void OpenCellWithNullCursor(Point point)//открытие всех пустых смежных ячеек
        {
            if (map[point] == -2)
            {
                map.CurrentMines++;
                map.ReShowNumberMines();
            }
            if (mines[point] == 0)
            {
                map[point] = -3;
                mines[point] = -3;
                map.WriteSymbol(point);
                for (int i = point.Number - 1; i < point.Number + 2; i++)
                {
                    for (int j = point.Letter - 1; j < point.Letter + 2; j++)
                    {
                        if (mines.IsInBorder(new Point(i, j)) && mines[new Point(i, j)] != -3)
                        {
                            OpenCellWithNullCursor(new Point(i, j));
                        }
                    }
                }
            }
            map[point] = mines[point];
            map.WriteSymbol(point);
        }
        private bool CheckWin(Map map)//проверка окончание игры победой
        {
            int CountFlags = 0;
            int CountEmptyCells = 0;
            for (int i = 0; i < SizeMapHeight; i++)//проход всех ячеек
            {
                for (int j = 0; j < SizeMapWidth; j++)
                {
                    if (map[i, j] == -2)
                    {
                        CountFlags++;
                    }
                    else if(map[i, j] == 0)
                    {
                        CountEmptyCells++;
                    }
                }
            }
            if (CountFlags == map.AmountOfMines && CountEmptyCells == 0) return true;
            return false;
        }
        private int ScoreCount(double time)//подсчет очков за игру
        {
            int score = (int)(200 * AmountOfMines / (double)SizeMapHeight / (double)SizeMapWidth);//очки за сложность
            score += (int)(CountCellWithNumber() / (Math.Log(time)));//очки за время
            return score;
        }
        private int CountCellWithNumber()//подсчет ячеек с цифрами (для определения очков за время)
        {
            int count = 0;
            for (int i = 0; i < SizeMapHeight; i++)
            {
                for (int j = 0; j < SizeMapWidth; j++)
                {
                    if (map[i, j] > 0) count++;
                }
            }
            return count;
        }
        public bool Exit()//выход
        {
            switch (Menu.ChooseExit())//меню выхода
            {
                case 2:
                    statistic.ShowLiders(Menu.ChooseComplexity());
                    break;
                case 3:
                    return true;
            }
            return false;
        }
    }
    public class Map//поле игры
    {
        public int SizeMapHeight { get; set; }
        public int SizeMapWidth { get; set; }
        int[,] map;//'*' = -1 - mine, '^' = -2 - flag, 1,2,3,4,5,6,7,8 - numbers
        public int AmountOfMines { get; set; }
        public int CurrentMines { get; set; }
        public Map(int sizeMapHeight, int sizeMapWidth, int amountOfMines)
        {
            SizeMapHeight = sizeMapHeight;
            SizeMapWidth = sizeMapWidth;
            AmountOfMines = CurrentMines = amountOfMines;
            map = new int[SizeMapHeight, SizeMapWidth];
        }
        public Map():this(9, 9, 10) { }
        public void InitialBombs(Point point)//point - координата первого хода //растановка бомб
        {
            Random rand = new Random();
            Point tempPoint = new Point();
            for (int i = 0; i < AmountOfMines; i++)
            {
                do
                {
                    tempPoint.Number = rand.Next(SizeMapHeight);
                    tempPoint.Letter = rand.Next(SizeMapWidth);
                } while (map[tempPoint.Number, tempPoint.Letter] == -1 || tempPoint.Equals(point));
                map[tempPoint.Number, tempPoint.Letter] = -1;
            }
            CountCellBomb();
        }
        private void CountCellBomb()//заполнение карты цифрами подсчета количества бомб вокруг ячейки
        {
            for (int i = 0; i < SizeMapHeight; i++)//поиск ячейки с бомбой
            {
                for (int j = 0; j < SizeMapWidth; j++)
                {
                    if (map[i, j] == -1)
                    {
                        CountBombs(new Point(i, j));
                    }
                }
            }
        }
        private void CountBombs(Point point)//проход ячеек вокруг бомбы с увеличением числа всех смежных ячеек на 1
        {
            for (int i = point.Number - 1; i < point.Number + 2; i++)
            {
                for (int j = point.Letter - 1; j < point.Letter + 2; j++)
                {
                    if (IsInBorder(new Point(i, j)))
                    {
                        if (map[i, j] != -1)
                        {
                            (map[i, j])++;
                        }
                    }
                }
            }
        }
        public bool IsInBorder(Point point)//проверка граници поля (для исключения изменения ячейки)
        {
            if (point.Number < 0 || point.Letter < 0 || point.Number > SizeMapHeight - 1 || point.Letter > SizeMapWidth - 1)
            {
                return false;//точка вне границ заданого поля
            }
            return true;//точка в границах заданого поля
        }
        public void Show()//отрисовка поля с минами
        {
            ShowBorderLetters();
            ShowHorizontalBorder();
            for (int i = 0; i < SizeMapHeight; i++)
            {
                ShowLeftBorder(i);
                for (int j = 0; j < SizeMapWidth; j++)
                {
                    Console.Write(GetSymbol(map[i, j]) + " ");
                    if (SizeMapWidth > 26) Console.Write(" ");
                }
                Console.WriteLine("|");
            }
            ShowHorizontalBorder();
            Menu.ShowSpaces((SizeMapHeight.ToString().Length + SizeMapWidth * (2 + SizeMapWidth / 26) 
                - 13 - AmountOfMines.ToString().Length) / 2);//отрисовка пробелов для центровки информации
            Console.WriteLine($"Оставшихся мин: {CurrentMines:00}\n");
        }
        public void ReShowNumberMines()//повторный показ колличества оставшихся мин (при управлении стрелками)
        {
            int number = 4 + SizeMapHeight;
            int letter = (SizeMapHeight.ToString().Length + SizeMapWidth * (2 + SizeMapWidth / 26) 
                - 13 - AmountOfMines.ToString().Length) / 2 + 16;
            Console.SetCursorPosition(letter, number);
            Console.Write("{0:00}", CurrentMines);
        }
        private void ShowLeftBorder(int indexRow)//отрисовка левой границы поля (с цифрами)
        {
            if (SizeMapHeight > 9 && indexRow < 9) Console.Write(" ");//если число строк двухзначное и номер строки меньше "10" добавляем пустую ячейку
            Console.Write($"{indexRow + 1}| ");
        }
        private void ShowBorderLetters()//отрисовка верхней линии с метками столбцов (буквы)
        {
            if (SizeMapHeight > 9) Console.Write(" ");
            Console.Write("  ");
            for (int i = 0; i < SizeMapWidth; i++)
            {
                if (i < 26) Console.Write(" ");
                else Console.Write((char)('a' + (i) / 26 - 1));
                Console.Write((char)('a' + i % 26));//literals: a,b,c,d...
                if (SizeMapWidth > 26) Console.Write(" ");
            }
            Console.WriteLine(" ");
        }
        private void ShowHorizontalBorder()//отрисовка горизонтальной граници поля с минами
        {
            if (SizeMapHeight > 9) Console.Write(" ");
            Console.Write(" +-");
            for (int i = 0; i < SizeMapWidth; i++)
            {
                Console.Write("--");
                if (SizeMapWidth > 26) Console.Write("-");
            }
            Console.WriteLine("+");
        }
        private char GetSymbol(int positionMap)//парсер с кода символа в символ char
        {
            switch (positionMap)
            {
                case -1:
                    return '*';//mine
                case -2:
                    return '^';//flag
                case 0:
                    return ' ';//empty
                case -3:
                    return (char)183;//empty (open)
                default:
                    return (char) (positionMap + 48);//1,2,3,4,5,6,7,8
            }
        }
        public int this[int i, int j]//доступ к ячейке поля
        {
            get
            {
                return map[i, j];
            }
            set
            {
                if (i < 0 || i >= SizeMapHeight || j < 0 || j >= SizeMapWidth) throw new IndexOutOfRangeException();
                map[i, j] = value;
            }
        }
        public int this[Point point]//доступ к ячейке поля
        {
            get
            {
                return map[point.Number, point.Letter];
            }
            set
            {
                map[point.Number, point.Letter] = value;
            }
        }
        private int XCursor(Point p) =>//определения позиции курсора по горизонтали (letter)
            SizeMapHeight.ToString().Length + 2 + (p.Letter) * (2 + SizeMapWidth / 26);
        private int YCursor(Point p) =>//определение позиции курсора по вертикали (number)
            3 + p.Number;
        public void WriteSymbol(Point p)//перезапись (отриовка) на поле ячейки
        {
            Console.SetCursorPosition(XCursor(p), YCursor(p));
            Console.Write(GetSymbol(map[p.Number, p.Letter]));
        }
        public void SetCursor(Point p)//установка курсора на экране по игровой координате
        {
            Console.SetCursorPosition(XCursor(p), YCursor(p));
        }
    }
    public struct Point
    {
        public int Letter { get; set; }
        public int Number { get; set; }
        public Point(int number, int letter)
        {
            Number = number;
            Letter = letter;
        }
        public Point Clone() => new Point(Number, Letter);
        public override string ToString() => $"{Number},{Letter}";
        public override bool Equals(object obj) => this.ToString().Equals(obj.ToString());
        public override int GetHashCode() => this.ToString().GetHashCode();
        public static bool operator ==(Point p1, Point p2) => p1.Equals(p2);
        public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
    }
    public static class Menu
    {
        public static Point EnterPoint(int sizeMapHeight, int SizeMapWidth)//ввод с клавиатуры координаты ячейки на поле Мар
        {
            Point point = new Point();
            string str;//строка для обработки
            int index = -1;//позиция в строке разделителя координат ','
            int number = sizeMapHeight + 1;//координата по высоте
            int letter = SizeMapWidth + 1;//координата в ширину (буква)
            do
            {
                Console.WriteLine("Введите координаты точки (пример 9,c)");
                str = Console.ReadLine();
                index = str.IndexOf(',');
                if (index != -1)
                {
                    int.TryParse(str.Substring(0, index), out number);
                    if (SizeMapWidth < 27 || str.Substring(index + 1).Length == 1) // если размер карты в ширину меньше количества букв или если ввели только 1 букву
                    {
                        letter = str.ToLower()[index + 1] - 'a' + 1;//переводим буквенный ввод (1 буква) в число
                    }
                    else letter = (str.ToLower()[index + 1] - 'a' + 1) * 26 + str.ToLower()[index + 2] - 'a' + 1;//переводим буквенный ввод (2 буквы) в число
                }
            } while (index == -1 || number > sizeMapHeight || letter > SizeMapWidth || number < 1 || letter < 1);
            point.Number = number - 1;
            point.Letter = letter - 1;
            return point;
        }
        public static int ChooseActionOnCell()//выбор действия для ячейки
        {
            int action = 0;
            do
            {
                Console.WriteLine("Введите:\n" +
                    "1 - открыть ячейку\n" +
                    "2 - поставить флаг на бомбу (^)");
                int.TryParse(Console.ReadLine(), out action);
            } while (action < 1 || action > 2);
            return action;
        }
        public static int ChooseExit()//проверка выхода из игры
        {
            int action = 0;
            do
            {
                Console.WriteLine("\nВведите:\n" +
                    "1 - начать заного\n" +
                    "2 - просмотреть таблицу лидеров\n" +
                    "3 - завершить игру");
                int.TryParse(Console.ReadLine(), out action);
            } while (action < 1 || action > 3);
            return action;
        }
        public static void ShowSpaces(int count)
        {
            if (count > 0) Console.Write(new string(' ', count));
        }
        public static int ChooseComplexity()//меню выбора сложности
        {
            int action = 0;
            do
            {
                Console.WriteLine("Выберите сложность:\n" +
                    "1 - новичок\n" +
                    "2 - любитель\n" +
                    "3 - профессионал\n" +
                    "4 - особый");
                int.TryParse(Console.ReadLine(), out action);
            } while (action < 1 || action > 4);
            return action;
        }
        public static int ChooseNewGameOrLiders()//стартовое меню
        {
            int action = 0;
            do
            {
                Console.WriteLine("Выберите:\n" +
                    "1 - новая игра\n" +
                    "2 - просмотреть таблицу лидеров");
                int.TryParse(Console.ReadLine(), out action);
            } while (action < 1 || action > 2);
            return action;
        }
        public static int EnterWidthMap()//ввод ширины поля (letter) при пользовательской игре
        {
            int width;
            do
            {
                Console.WriteLine("Введите ширину поля:");
            } while (!int.TryParse(Console.ReadLine(), out width));
            return width;
        }
        public static int EnterHeightMap()//ввод высоты поля (number) при пользовательской игре
        {
            int height;
            do
            {
                Console.WriteLine("Введите высоту поля:");
            } while (!int.TryParse(Console.ReadLine(), out height));
            return height;
        }
        public static int EnterAmountOfMines()//ввод колличества мин при пользовательской игре
        {
            int mines;
            do
            {
                Console.WriteLine("Введите колличество мин:");
            } while (!int.TryParse(Console.ReadLine(), out mines));
            return mines;
        }
    }
    public class User
    {
        public string Name { get; protected set; }
        private int score;
        public int Score
        {
            get => score;
            set
            {
                if (value >= 0) score = value;
                else throw new ArgumentException("Cчет не может быть отрицательным");
            }
        }
        public User (string name, int score)
        {
            Name = name;
            Score = score;
        }
        public User() : this("User", 0) { }
        public void Show(int widthMap, int heightMap)//отрисовка на поле данных пользователя
        {
            Menu.ShowSpaces((heightMap.ToString().Length + widthMap * (2 + widthMap / 26) 
                - 11 - (Name+score.ToString()).Length) / 2);//отрисовка пробелов для центровки информации
            Console.WriteLine($"Игрок: {Name} очки: {Score}");
        }
    }
    public class Statistic//данные статистики игры
    {
        private List<PlayedGame> newbie;
        private List<PlayedGame> amateur;
        private List<PlayedGame> professional;
        private List<PlayedGame> special;
        public Statistic()
        {
            newbie = new List<PlayedGame>(11);
            amateur = new List<PlayedGame>(11);
            professional = new List<PlayedGame>(11);
            special = new List<PlayedGame>(11);
        }
        private void AddNewbie(PlayedGame game)//добавление записи в таблицу "новичек"
        {
            if (newbie.Count == 0 || game.TimeGame < newbie.LastOrDefault().TimeGame) 
            {
                newbie.Add(game);
                newbie = newbie.OrderBy(t => t.TimeGame).ToList();
                if (newbie.Count == 11)
                {
                    newbie.RemoveAt(10);
                }
            }
        }
        private void AddAmateur(PlayedGame game)//добавление записи в таблицу "любитель"
        {
            if (amateur.Count == 0 || game.TimeGame < amateur.LastOrDefault().TimeGame)
            {
                amateur.Add(game);
                amateur = amateur.OrderBy(t => t.TimeGame).ToList();
                if (amateur.Count == 11)
                {
                    amateur.RemoveAt(10);
                }
            }
        }
        private void AddProfesional(PlayedGame game)//добавление записи в таблицу "профессионал"
        {
            if (professional.Count == 0 || game.TimeGame < professional.LastOrDefault().TimeGame)
            {
                professional.Add(game);
                professional = professional.OrderBy(t => t.TimeGame).ToList();
                if (professional.Count == 11)
                {
                    professional.RemoveAt(10);
                }
            }
        }
        private void AddSpecial(PlayedGame game)//добавление записи в таблицу "особое"
        {
            if (special.Count == 0 || game.ScoreGame > special.LastOrDefault().ScoreGame)
            {
                special.Add(game);
                special = special.OrderByDescending(t => t.ScoreGame).ToList();
                if (special.Count == 11)
                {
                    special.RemoveAt(10);
                }
            }
        }
        public void Add(int complexity, string name, int score, double time, DateTime date)//добавление данных в таблици лидеров
        {
            if (complexity < 1 || complexity > 4) throw new ArgumentOutOfRangeException();
            if (name == null) throw new ArgumentNullException();
            switch (complexity)
            {
                case 1:
                    AddNewbie(new PlayedGame(name, score, time, date));
                    break;
                case 2:
                    AddAmateur(new PlayedGame(name, score, time, date));
                    break;
                case 3:
                    AddProfesional(new PlayedGame(name, score, time, date));
                    break;
                case 4:
                    AddSpecial(new PlayedGame(name, score, time, date));
                    break;
            }
        }
        public void ShowLiders(int complexity)//отрисовка таблици лидеров
        {
            if (complexity < 1 || complexity > 4) throw new ArgumentOutOfRangeException();
            Console.Clear();
            int i = 1;
            switch (complexity)
            {
                case 1:
                    Console.WriteLine("\tТаблица лидеров сложности \"Новичек\"");
                    foreach (PlayedGame item in newbie)
                    {
                        Console.WriteLine($"{i++}. {item}");
                    }
                    break;
                case 2:
                    Console.WriteLine("\tТаблица лидеров сложности \"Любитель\"");
                    foreach (PlayedGame item in amateur)
                    {
                        Console.WriteLine($"{i++}. {item}");
                    }
                    break;
                case 3:
                    Console.WriteLine("\tТаблица лидеров сложности \"Профессионал\"");
                    foreach (PlayedGame item in professional)
                    {
                        Console.WriteLine($"{i++}. {item}");
                    }
                    break;
                case 4:
                    Console.WriteLine("\tТаблица лидеров сложности \"Особое\"");
                    foreach (PlayedGame item in special)
                    {
                        Console.WriteLine($"{i++}. {item}");
                    }
                    break;
            }
            Console.Write("Нажмите клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    public struct PlayedGame
    {
        public string NamePlayer { get; private set; }
        public int ScoreGame { get; private set; }
        public double TimeGame { get; private set; }
        public DateTime Date { get; private set; }
        public PlayedGame(string name, int score, double time, DateTime date)
        {
            NamePlayer = name;
            ScoreGame = score;
            TimeGame = time;
            Date = date;
        }
        public override string ToString() => 
            $"Игрок: {NamePlayer} время {TimeGame:0.00} очки {ScoreGame} дата {Date:G}";
    }
}
