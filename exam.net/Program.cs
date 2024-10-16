using System;
using System.Diagnostics;

namespace SudokuGame
{
    public struct CellPosition
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public CellPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
    public interface ISudokuRules
    {
        bool IsValid(int num, int row, int col);
        bool IsSudokuComplete();
    }

    public class SudokuRules : ISudokuRules
    {
        private readonly int[,] _grid;
        private const int GridSize = 9;
        private const int SquareSize = 3;

        public SudokuRules(int[,] grid)
        {
            _grid = grid;
        }

        public bool IsValid(int num, int row, int col)
        {
            return IsRowValid(num, row) && IsColValid(num, col) && IsSquareValid(num, row, col);
        }

        public bool IsSudokuComplete()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (_grid[row, col] == 0)
                        return false;
                }
            }
            return true;
        }

        private bool IsRowValid(int num, int row)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (_grid[row, col] == num)
                    return false;
            }
            return true;
        }

        private bool IsColValid(int num, int col)
        {
            for (int row = 0; row < GridSize; row++)
            {
                if (_grid[row, col] == num)
                    return false;
            }
            return true;
        }

        private bool IsSquareValid(int num, int row, int col)
        {
            int startRow = row / SquareSize * SquareSize;
            int startCol = col / SquareSize * SquareSize;

            for (int r = startRow; r < startRow + SquareSize; r++)
            {
                for (int c = startCol; c < startCol + SquareSize; c++)
                {
                    if (_grid[r, c] == num)
                        return false;
                }
            }
            return true;
        }
    }

    public class SudokuGridManager
    {
        private const int GridSize = 9;
        public int[,] Grid { get; private set; }
        private Random rand = new Random();

        public SudokuGridManager()
        {
            Grid = new int[GridSize, GridSize];
        }

        public void ClearGrid()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Grid[row, col] = 0;
                }
            }
        }

        public void SetValue(int num, int row, int col)
        {
            Grid[row, col] = num;
        }

        public int GetValue(int row, int col)
        {
            return Grid[row, col];
        }

        public void GeneratePuzzle()
        {
            FillGrid();
            RemoveRandomCells();
        }

        private bool FillGrid()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (Grid[row, col] == 0)
                    {
                        int[] numbers = GetShuffledNumbers();
                        foreach (var num in numbers)
                        {
                            if (new SudokuRules(Grid).IsValid(num, row, col))
                            {
                                Grid[row, col] = num;
                                if (FillGrid())
                                    return true;
                                Grid[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void RemoveRandomCells()
        {
            int cellsToRemove = 40;
            while (cellsToRemove > 0)
            {
                int row = rand.Next(0, GridSize);
                int col = rand.Next(0, GridSize);
                if (Grid[row, col] != 0)
                {
                    Grid[row, col] = 0;
                    cellsToRemove--;
                }
            }
        }

        private int[] GetShuffledNumbers()
        {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = numbers.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }
            return numbers;
        }
    }

    public delegate void CellFilledEventHandler(CellPosition pos, int number);

    class Program
    {
        const int GridSize = 9;
        static SudokuGridManager gridManager = new SudokuGridManager();
        static ISudokuRules rules;
        static Stopwatch stopwatch = new Stopwatch();

        public static event CellFilledEventHandler CellFilled;

        static void Main(string[] args)
        {
            CellFilled += OnCellFilled;
            MainMenu();
        }

        static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("|------------------------------------------|");
                Console.WriteLine("добро пожаловать в игру Судоку!)");
                Console.WriteLine("1. Начать игру");
                Console.WriteLine("2. Правила игры");
                Console.WriteLine("3. Информация о разработчике");
                Console.WriteLine("4. Выйти из игры");
                Console.Write("Выберите пункт меню: ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StartGame();
                        break;
                    case "2":
                        ShowRules();
                        break;
                    case "3":
                        ShowDeveloperInfo();
                        break;
                    case "4":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("неверный выбор,нажмите любую клавишу для продолжения");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void StartGame()
        {
            gridManager.ClearGrid();
            gridManager.GeneratePuzzle();
            rules = new SudokuRules(gridManager.Grid);
            stopwatch.Restart();
            PlayGame();
        }

        static void ShowRules()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("правила игры:");
            Console.WriteLine("1. цель игры — заполнить все клетки числам от 1 до 9");
            Console.WriteLine("2. числа не должны повторяться в строках, столбцах и в квадратах 3x3");
            Console.WriteLine("нажмите любую клавишу для возврата в меню");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void ShowDeveloperInfo()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("разработчик: Юсипiв Олександр");
            Console.WriteLine("контакты: yusypiv07@gmail.com");
            Console.WriteLine("нажмите любую клавишу для возврата в меню");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void PlayGame()
        {
            while (true)
            {
                Console.Clear();
                PrintSudokuGrid();
                Console.WriteLine($"время игры: {stopwatch.Elapsed.Minutes} мин {stopwatch.Elapsed.Seconds} сек");

                Console.WriteLine("введите строку от 0 до 8: ");
                int row = int.Parse(Console.ReadLine());

                Console.WriteLine("введите столбец от 0 до 8: ");
                int col = int.Parse(Console.ReadLine());

                Console.WriteLine("введите число от 1 до 9: ");
                int num = int.Parse(Console.ReadLine());

                CellPosition pos = new CellPosition(row, col);
                CellFilled?.Invoke(pos, num);
            }
        }

        static void OnCellFilled(CellPosition pos, int num)
        {
            if (rules.IsValid(num, pos.Row, pos.Col))
            {
                gridManager.SetValue(num, pos.Row, pos.Col);
                if (rules.IsSudokuComplete())
                {
                    stopwatch.Stop();
                    Console.Clear();
                    PrintSudokuGrid();
                    Console.WriteLine($"поздравляем! вы решили судоку за {stopwatch.Elapsed.Minutes} мин {stopwatch.Elapsed.Seconds} сек.");
                    Console.ReadLine();
                    MainMenu();
                }
            }
            else
            {
                Console.WriteLine("неверный ход,попробуйте еще раз");
                Console.ReadLine();
            }
        }

        static void PrintSudokuGrid()
        {
            Console.WriteLine("  +-------+-------+-------+");
            for (int row = 0; row < GridSize; row++)
            {
                if (row % 3 == 0 && row != 0)
                    Console.WriteLine("  +-------+-------+-------+");

                Console.Write("  | ");
                for (int col = 0; col < GridSize; col++)
                {
                    int val = gridManager.GetValue(row, col);
                    Console.Write(val == 0 ? ". " : val + " ");

                    if ((col + 1) % 3 == 0)
                        Console.Write("| ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("  +-------+-------+-------+");
        }
    }
}