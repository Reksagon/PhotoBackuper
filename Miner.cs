﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class Miner
    {
        public int SizeMapHeight { get; set; }
        public int SizeMapWidth { get; set; }
        Map mines;//'*' = -1 - mine, '^' = -2 - flag, 1,2,3,4,5,6,7,8 - numbers
        Map map;
        public int AmountOfMines { get; set; }
        public Miner()
        {
            SizeMapHeight = 8;
            SizeMapWidth = 8;
            AmountOfMines = 10;
        }
        private void Initial()
        {
            mines = new Map(SizeMapHeight, SizeMapWidth, AmountOfMines);
            map = new Map(SizeMapHeight, SizeMapWidth, AmountOfMines);
        }
        public void Start()
        {
            bool endGame = false;
            bool bombed = false;
            Point point = new Point();
            bool firstMove = true;
            do
            {
                Initial();
                endGame = false;
                bombed = false;
                firstMove = true;
                do
                {
                    Console.Clear();
                    map.Show();
                    point = Menu.EnterPoint(SizeMapHeight, SizeMapWidth);
                    if (Menu.ChooseActionOnCell() == 1)
                    {
                        if (firstMove)
                        {
                            mines.InitialBombs(point);
                            firstMove = false;
                        }
                        bombed = OpenCell(point);
                    }
                    else
                    {
                        map[point] = -2;
                    }
                    if (bombed)
                    {
                        Console.WriteLine("Поражение!!!");
                        endGame = true;
                    }
                    else if (CheckWin(map))
                    {
                        Console.WriteLine("Победа!!!");
                        endGame = true;
                    }
                } while (!endGame);
            } while (!Menu.ChooseExit());
        }
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
        private void OpenCellWithNull(Point point)//открытие всех пустых смежных ячеек
        {
            if (mines[point] == 0)
            {
                map[point] = 0;
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
    }
    public class Map
    {
        public int SizeMapHeight { get; set; }
        public int SizeMapWidth { get; set; }
        int[,] map;//'*' = -1 - mine, '^' = -2 - flag, 1,2,3,4,5,6,7,8 - numbers
        public int AmountOfMines { get; set; }
        public Map(int sizeMapHeight, int sizeMapWidth, int amountOfMines)
        {
            SizeMapHeight = sizeMapHeight;
            SizeMapWidth = sizeMapWidth;
            AmountOfMines = amountOfMines;
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
        private void ShowHorizontalBorder()
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
        private char GetSymbol(int positionMap)
        {
            switch (positionMap)
            {
                case -1:
                    return '*';//mine
                case -2:
                    return '^';//flag
                case 0:
                case -3:
                    return ' ';//empty
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
        public override string ToString()
        {
            return $"{Number},{Letter}";
        }
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }
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
        public static bool ChooseExit()//проверка выхода из игры
        {
            int action = 0;
            do
            {
                Console.WriteLine("\nВведите:\n" +
                    "1 - начать заного\n" +
                    "2 - завершить игру");
                int.TryParse(Console.ReadLine(), out action);
            } while (action < 1 || action > 2);

            return action == 2 ? true : false;
        }
    }
}
