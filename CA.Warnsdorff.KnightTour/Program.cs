using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace CA.Warnsdorff.KnightTour
{
    class Program
    {
        static Random rnd = new Random();
        static string _title = "The Knight's tour problem using Warnsdorff's algorithm";
        static int _boardSize = 8;
        static int[,] _board;
        static int _emptyVal = -1;
        static int[] _rowDeltas = new int[8] { 1, -1, -2, -2, -1, 1, 2, 2 };
        static int[] _colDeltas = new int[8] { -2, -2, -1, 1, 2, 2, 1, -1 };
        static int _initRow = 0;
        static int _initCol = 0;
        static int _initStep = 1;
        static int _numRecursiveCalls = 0;

        static void Main(string[] args)
        {
            Console.WriteLine($"::: {_title} :::\n\r");

            CreateBoard();

            _board[_initRow, _initCol] = _initStep;
            if (Warnsdorff(_initRow, _initCol, _initStep))
            {
                Console.WriteLine($"One solution after {_numRecursiveCalls} recursive calls:\n\r");
                PrintBoard();
            }
            else
            {
                Console.WriteLine("No found solution for this combination.\n\r");
            }

        }

        static void CreateBoard()
        {
            _board = new int[_boardSize, _boardSize];

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    _board[i, j] = _emptyVal;
                }
            }
            Console.WriteLine($"Initiallized {_boardSize}x{_boardSize} board.");
        }

        static bool IsValid(int row, int col)
        {
            return ((row >= 0) && (col >= 0) && (row < _boardSize) && (col < _boardSize) && (_board[row, col] == _emptyVal));
        }

        static (List<int> neighsRow, List<int> neighsCol) FindSafeNeighborsWithMinDegree(int row, int col)
        {
            List<int> neighsMinRow = new List<int>();
            List<int> neighsMinCol = new List<int>();

            // Find Valid Neighbors
            List<int> neighsRow = new List<int>();
            List<int> neighsCol = new List<int>();
            List<int> degrees = new List<int>();
            int neighRow, neighCol, degree;
            for (int i = 0; i < _rowDeltas.Length; i++)
            {
                neighRow = row + _rowDeltas[i];
                neighCol = col + _colDeltas[i];
                if (IsValid(neighRow, neighCol))
                {
                    neighsRow.Add(neighRow);
                    neighsCol.Add(neighCol);
                    degree = DegreeOf(neighRow, neighCol);
                    degrees.Add(degree);
                }
            }

            // Find the minimum degree among all valid neighbors
            int minDegree = 0;
            if (degrees.Count > 0)
                minDegree = degrees.Min();

            // Find all neighbors with the same min degree
            for (int i = 0; i < neighsRow.Count; i++)
            {
                if (degrees.ElementAt(i) == minDegree)
                {
                    neighsMinRow.Add(neighsRow.ElementAt(i));
                    neighsMinCol.Add(neighsCol.ElementAt(i));
                }
            }
            return (neighsMinRow, neighsMinCol);
        }

        private static int DegreeOf(int row, int col)
        {
            int total = 0;
            int neighRow, neighCol;
            for (int i = 0; i < _rowDeltas.Length; i++)
            {
                neighRow = row + _rowDeltas[i];
                neighCol = col + _colDeltas[i];
                if (IsValid(neighRow, neighCol))
                    total++;
            }
            return total;
        }

        static bool Warnsdorff(int row, int col, int step)
        {
            //PrintBoard(); Console.WriteLine();
            _numRecursiveCalls++;
            if (step == (_boardSize * _boardSize) - 1 + _initStep)
                return true;
            // Find neighbors with minimum degree
            (List<int> neighsRow, List<int> neighsCol) = FindSafeNeighborsWithMinDegree(row, col);

            // Generate Shuffled indexes to select neighbors at random
            // This avoids the program to create the same route in every run
            int[] sequence = new int[neighsRow.Count];
            for (int i = 0; i < neighsRow.Count; i++)
            {
                sequence[i] = i;
            }
            int[] shuffledIndexes = sequence.OrderBy(x => rnd.Next()).ToArray();

            // Try to complete the board with the going to the first neigh, otherwise the second...
            for (int i = 0; i < neighsRow.Count; i++)
            {
                _board[neighsRow[shuffledIndexes[i]], neighsCol[shuffledIndexes[i]]] = ++step;
                if (Warnsdorff(neighsRow[shuffledIndexes[i]], neighsCol[shuffledIndexes[i]], step))
                    return true;
                _board[neighsRow[shuffledIndexes[i]], neighsCol[shuffledIndexes[i]]] = _emptyVal;
                step--;
            }
            // If no valid route has been found return false to revert this path
            return false;

        }

        static void PrintBoard(char marker = '*')
        {
            int maxDigits = MaxBoardVal().ToString().Count();
            string markers = new String(marker, maxDigits);

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (_board[i, j] == _emptyVal)
                        Console.Write(markers+" ");
                    else
                    {
                        int digits = _board[i, j].ToString().Count();
                        string spaces = new String(' ', maxDigits - digits);
                        Console.Write($"{_board[i, j]}" + spaces + " ");
                    }
                }
                Console.WriteLine();
            }
        }
        static int MaxBoardVal()
        {
            int max = -1;
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (_board[i, j] > max)
                        max = _board[i, j];
                }
            }
            return max;
        }

    }
}
