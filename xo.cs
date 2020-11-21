using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TicTacToe
{
	class Program
	{
		delegate void method();
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(".-----. _         .-----.             .-----.            ");
			Console.WriteLine("`-. .-':_;        `-. .-'             `-. .-'            ");
			Console.WriteLine("  : :  .-. .--.     : : .--.   .--.     : : .--.  .--.   ");
			Console.WriteLine("  : :  : :'  ..'    : :' .; ; '  ..'    : :' .; :' '_.'  ");
			Console.WriteLine("  :_;  :_;`.__.'    :_;`.__,_;`.__.'    :_;`.__.'`.__.'  ");
			Console.ResetColor();
			StarWars();
			Start();
		}

		
		static int count = 0; // кол-во игр
		static int x_win = 0; // кол-во побед х
		static int o_win = 0; // кол-во обед о
		static int x_pts = 0; // кол-во птс
		static int o_pts = 0; // кол-во птс
		static int y = 0; // коеф птс за победу
		private static void PlayGame()
		{
			string Rows = null;
			while (Rows == null)
			{
				string[] items = { "Обычный (3х3)", "Средний (4х4)", "Высокий (5х5)", "Назад"};
				method[] methods = new method[] { Usual, Middle, Complicated, Back};
				ConsoleMenu menu = new ConsoleMenu(items);
				int menuResult;
				do
				{
					menuResult = menu.PrintMenu();
					methods[menuResult]();
				}
				while (menuResult != items.Length - 1);
			}
			var boardSize = 0;
			void Usual()
			{
				boardSize = (int)Math.Pow(int.Parse("3"), 2);
				y = 9;
				Game();
			}
			void Middle()
			{
				boardSize = (int)Math.Pow(int.Parse("4"), 2);
				y = 16;
				Game();
			}
			void Complicated()
			{
				boardSize = (int)Math.Pow(int.Parse("5"), 2);
				y = 25;
				Game();
			}
			void Back()
			{
				Start();
			}

			void Game()
			{
				Random rnd = new Random();
				
				var board = new string[boardSize];
				var move = "X";
				while (true)
				{
					Console.Clear();

					var winner = Win(board);
					if (winner != null)
					{
						count++;
						if (winner[0] == 'X')
						{
							x_win++;
							x_pts += y;

						}
						else if (winner[0] == 'O')
						{
							o_win++;
							o_pts += y;
						}
						Result("\t" + winner[0] + " ПОБЕДА!!!", board);
						break;
					}
					if (Draw(board))
					{
						count++;
						x_pts += 5;
						o_pts += 5;
						Result("\tНИЧЬЯ!!!", board);
						break;
					}


					if (move == "X")
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("Ваш ход:");
						Console.ResetColor();

						DrawBoard(board);

						var XO = GetXOLocation(board);
						board[XO] = move;
					}

					if (move == "O")
					{
						while (true)
						{

							var XO = rnd.Next(0, y);
							if (board[XO] == null)
							{
								board[XO] = move;
								break;
							}
						}
					}
					move = move == "O" ? "X" : "O";
				}
			}
		}
		private static void Start()
		{
			var stillPlaying = true;
			while (stillPlaying)
			{
				string[] items = { "Начать новую игру", "Статистика", "Выход" };
				method[] methods = new method[] { Method1, Method2, Exit };
				ConsoleMenu menu = new ConsoleMenu(items);
				int menuResult;
				do
				{
					menuResult = menu.PrintMenu();
					methods[menuResult]();
				} while (menuResult != items.Length - 1);
				void Method1()
				{
					PlayGame();
				}
				void Method2()
				{
					Statistics();
				}
				void Exit() // НЕ ХОЧЕТ ВЫХОДИТЬ
				{
					stillPlaying = false;
				}
			}

		}
		private static void StarWars()
		{
			Console.Beep(300, 500);
			Thread.Sleep(50);
			Console.Beep(300, 500);
			Thread.Sleep(50);
			Console.Beep(300, 500);
			Thread.Sleep(50);
			Console.Beep(250, 500);
			Thread.Sleep(50);
			Console.Beep(350, 250);
			Console.Beep(300, 500);
			Thread.Sleep(50);
			Console.Beep(250, 500);
			Thread.Sleep(50);
			Console.Beep(350, 250);
			Console.Beep(300, 500);
			Thread.Sleep(50);
		}

		private static void Statistics()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine("Всего игр: " + count);
			Console.WriteLine("Побед Х: " + x_win + " = " + x_pts + " очков");
			Console.WriteLine("Побед О: " + o_win + " = " + o_pts + " очков");
			Console.Write("Нажмите любую клавишу...");
			Console.ResetColor();
			Console.CursorVisible = false;
			Console.ReadKey();
			Console.CursorVisible = true;
			Console.ResetColor();
			Console.Clear();
		}
		private static void Result(string message, string[] board)
		{
			Console.WriteLine();
			DrawBoard(board);

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.Write("Нажмите любую клавишу...");
			Console.ResetColor();
			Console.CursorVisible = false;
			Console.ReadKey();
			Console.CursorVisible = true;
		}

		private static int GetXOLocation(string[] board)
		{
			
			int numRows = (int)Math.Sqrt(board.Length);

			int curRow = 0, curCol = 0;

			for (int i = 0; i < board.Length; i++)
			{
				if (board[i] == null)
				{
					curRow = i / numRows;
					curCol = i % numRows;
					break;
				}
			}
			
			while (true)
			{
				Console.SetCursorPosition(curCol * 4 + 2, curRow * 4 + 3);
				var keyInfo = Console.ReadKey();
				Console.SetCursorPosition(curCol * 4 + 2, curRow * 4 + 3);
				Console.Write(board[curRow * numRows + curCol] ?? " ");

				switch (keyInfo.Key)
				{
					case ConsoleKey.LeftArrow:
						if (curCol > 0)
							curCol--;
						break;
					case ConsoleKey.RightArrow:
						if (curCol + 1 < numRows)
							curCol++;
						break;
					case ConsoleKey.UpArrow:
						if (curRow > 0)
							curRow--;
						break;
					case ConsoleKey.DownArrow:
						if (curRow + 1 < numRows)
							curRow++;
						break;
					case ConsoleKey.Spacebar:
					case ConsoleKey.Enter:
						if (board[curRow * numRows + curCol] == null)
							return curRow * numRows + curCol;
						break;
				}
			}
		}

		private static void DrawBoard(string[] board)
		{
			var row = (int)Math.Sqrt(board.Length);

			Console.WriteLine();

			for (int i = 0; i < row; i++)
			{
				if (i != 0)
					Console.WriteLine(" " + string.Join("|", Enumerable.Repeat("---", row)));

				Console.Write(" " + string.Join("|", Enumerable.Repeat("   ", row)) + "\n ");

				for (int j = 0; j < row; j++)
				{
					if (j != 0)
						Console.Write("|");
					var space = board[i * row + j] ?? " ";
					if (space.Length > 1)
						Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(" " + space[0] + " ");
					Console.ResetColor();
				}

				Console.WriteLine("\n " + string.Join("|", Enumerable.Repeat("   ", row)));
			}

			Console.WriteLine();
		}

		private static bool Draw(IEnumerable<string> board)
		{
			return board.All(space => space != null);
		}

		private static string Win(string[] board)
		{
			var row = (int)Math.Sqrt(board.Length);
			// Проверяем ряды
			for (int i = 0; i < row; i++)
			{
				if (board[i * row] != null)
				{
					bool hasTicTacToe = true;
					for (int col = 1; col < row && hasTicTacToe; col++)
					{
						if (board[i * row + col] != board[i * row])
							hasTicTacToe = false;
					}
					if (hasTicTacToe)
					{
						for (int col = 0; col < row; col++)
							board[i * row + col] += "!";
						return board[i * row];
					}
				}
			}

			// Проверяем колонки
			for (int i = 0; i < row; i++)
			{
				if (board[i] != null)
				{
					bool hasTicTacToe = true;
					for (int j = 1; j < row && hasTicTacToe; j++)
					{
						if (board[j * row + i] != board[i])
							hasTicTacToe = false;
					}
					if (hasTicTacToe)
					{
						for (int k = 0; k < row; k++)
							board[k * row + i] += "!";
						return board[i];
					}
				}
			}

			// Проверяем диагональ \
			if (board[0] != null)
			{
				bool hasTicTacToe = true;
				for (int i = 1; i < row && hasTicTacToe; i++)
				{
					if (board[i * row + i] != board[0])
						hasTicTacToe = false;
				}
				if (hasTicTacToe)
				{
					for (int j = 0; j < row; j++)
						board[j * row + j] += "!";
					return board[0];
				}
			}

			// Проверяем диагональ /
			if (board[row - 1] != null)
			{
				bool hasTicTacToe = true;
				for (int i = 1; i < row && hasTicTacToe; i++)
				{
					if (board[i * row + (row - 1 - i)] != board[row - 1])
						hasTicTacToe = false;
				}
				if (hasTicTacToe)
				{
					for (int j = 0; j < row; j++)
						board[j * row + (row - 1 - j)] += "!";
					return board[row - 1];
				}
			}

			return null;
		}
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
						Console.BackgroundColor = ConsoleColor.Yellow;
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
