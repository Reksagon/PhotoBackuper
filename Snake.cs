using System;

namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Snake obj = new Snake();
            obj.Show();
            Console.ReadKey();
        }

    }
    class Snake
    {
        public Snake() { }
        public void Show()
        {
            Console.WriteLine("Hello, it's new game");
        }
    }
}
