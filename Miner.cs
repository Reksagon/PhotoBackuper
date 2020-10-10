using System;
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
        public void Initial(Point point)//point - first move
        {
            Random rand = new Random();
            Point tempPoint = new Point();
            for (int i = 0; i < AmountOfMines; i++)
            {
                do
                {
                    tempPoint.Number = rand.Next(0, SizeMapHeight - 1);
                    tempPoint.Letter = rand.Next(0, SizeMapWidth - 1);
                } while (map[tempPoint.Number, tempPoint.Letter] == -1 || tempPoint.ToString().Equals(point.ToString()));
                map[tempPoint.Number, tempPoint.Letter] = -1;
            }
            CountCellBomb();
        }
        private void CountCellBomb()
        {
            for (int i = 0; i < SizeMapHeight; i++)
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
        private void CountBombs(Point point)
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
                            //map[i, j] = map[i, j] + 1;
                        }
                    }
                }
            }
        }
        private bool IsInBorder(Point point)
        {
            if (point.Number < 0 || point.Letter < 0 || point.Number > SizeMapHeight || point.Letter > SizeMapWidth)
            {
                return false;
            }
            return true;
        }
        public void Show()
        {
            if (SizeMapHeight > 9) Console.Write(" ");
            Console.Write("   ");
            for (int i = 0; i < SizeMapWidth; i++)
            {
                Console.Write($"{(char)(i + 97)} ");//literals: a,b,c,d...
            }
            Console.WriteLine(" ");
            ShowHorizontalLine();
            for (int i = 0; i < SizeMapHeight; i++)
            {
                if (SizeMapHeight > 9 && i < 9) Console.Write(" ");
                Console.Write($"{i + 1}| ");
                for (int j = 0; j < SizeMapWidth; j++)
                {
                    Console.Write(GetSymbol(map[i, j]) + " ");
                }
                Console.WriteLine("|");
            }
            ShowHorizontalLine();
        }
        private void ShowHorizontalLine()
        {
            if (SizeMapHeight > 9) Console.Write(" ");
            Console.Write(" +-");
            for (int i = 0; i < SizeMapWidth; i++)
            {
                Console.Write("--");
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
                    return ' ';//empty
                default:
                    return (char) (positionMap + 48);//1,2,3,4,5,6,7,8
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
    }
}
