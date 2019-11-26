using System;

namespace AdventOfCodeTools
{
    // TODO more operations with Vector
    public class Grid<T>
    {
        private T[,] m_Grid;
        public int xLength { get => m_Grid.GetLength(0); }
        public int yLength { get => m_Grid.GetLength(1); }

        public Grid(int xLength, int yLength)
        {
            m_Grid = new T[xLength, yLength];
        }

        public T[] GetXRow(int y)
        {
            var result = new T[xLength];

            for (var x = 0; x < xLength; x++)
            {
                result[x] = m_Grid[x, y];
            }

            return result;
        }

        public void SetXRow(T[] row, VectorInt yList)
        {
            for (var index = 0; index < yList.Length; index++)
            {
                var y = yList[index];
                for (var x = 0; x < xLength; x++)
                {
                    m_Grid[x, y] = row[x];
                }
            }
        }

        public T[] GetYRow(int x)
        {
            var result = new T[yLength];

            for (var y = 0; y < yLength; y++)
            {
                result[y] = m_Grid[x, y];
            }

            return result;
        }

        public void SetYRow(T[] row, VectorInt xList)
        {
            for (var index = 0; index < xList.Length; index++)
            {
                var x = xList[index];
                for (var y = 0; y < yLength; y++)
                {
                    m_Grid[x, y] = row[y];
                }
            }
        }

        public T[] Flatten()
        {
            var flat = new T[xLength * yLength];

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    flat[x * yLength + y] = m_Grid[x, y];
                }
            }
            return flat;
        }

        public Grid<T> GetSubgrid(VectorInt xList, VectorInt yList)
        {
            var nbX = xList.Length;
            var nbY = yList.Length;

            var result = new Grid<T>(nbX, nbY);

            for (var i = 0; i < nbX; i++)
            {
                var x = xList[i];
                for (var j = 0; j < nbY; j++)
                {
                    var y = yList[j];
                    result[i, j] = this[x, y];
                }
            }

            return result;
        }

        public T this[int x, int y]
        {
            get => m_Grid[x, y];
            set => m_Grid[x, y] = value;
        }

        // TODO compute sub grids
        // TODO improve the perf by using string builder ?
        public void Print(Action<T, int, int> cellPrinter, bool reverseY = false)
        {
            for (var y = 0; y < yLength; y++)
            {
                for (var x = 0; x < xLength; x++)
                {
                    var yValue = reverseY ? yLength - y - 1 : y;
                    cellPrinter?.Invoke(m_Grid[x, yValue], x, yValue);
                }
                Console.WriteLine();
            }
        }

        public void Print(bool reverseY = false)
        {
            Print((val, x, y) => { IO.Print(val.ToString()); }, reverseY);
        }
    }
}
