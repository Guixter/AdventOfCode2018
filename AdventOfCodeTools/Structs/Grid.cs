using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCodeTools
{
    public class Grid<T>
    {
        public static Grid<T> Constant(int xLength, int yLength, T value)
        {
            var result = new Grid<T>(xLength, yLength);

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    result[x, y] = value;
                }
            }

            return result;
        }

        public static Grid<T> Constant(int size, T value)
        {
            return Constant(size, size, value);
        }

        public static Grid<T> Diagonal(Vector<T> values)
        {
            var size = values.Length;
            var result = new Grid<T>(size, size);

            for (var i = 0; i < size; i++)
            {
                result[i, i] = values[i];
            }

            return result;
        }

        public static Grid<T> Diagonal(int size, T value)
        {
            return Diagonal(Vector<T>.Constant(size, value));
        }


        internal T[,] m_Grid;
        public int xLength { get => m_Grid.GetLength(0); }
        public int yLength { get => m_Grid.GetLength(1); }
        public VectorInt AllX => VectorInt.Incremental(xLength);
        public VectorInt AllY => VectorInt.Incremental(yLength);


        public Grid(int xLength, int yLength)
        {
            m_Grid = new T[xLength, yLength];
        }

        public Grid(int xLength, int yLength, params T[] values) : this(xLength, yLength)
        {
            Debug.Assert(values.Length == xLength * yLength);

            for (var i = 0; i < values.Length; i++)
            {
                var x = i % xLength;
                var y = i / xLength;
                m_Grid[x, y] = values[i];
            }
        }

        internal Grid(T[,] grid)
        {
            m_Grid = grid;
        }


        public T this[int x, int y]
        {
            get => m_Grid[x, y];
            set => m_Grid[x, y] = value;
        }

        public Grid<T> this[VectorInt xList, VectorInt yList]
        {
            get
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
            set
            {
                var nbX = xList.Length;
                var nbY = yList.Length;

                for (var i = 0; i < nbX; i++)
                {
                    var x = xList[i];
                    for (var j = 0; j < nbY; j++)
                    {
                        var y = yList[j];
                        this[x, y] = value[i, j];
                    }
                }
            }
        }

        public Vector<T> this[VectorInt xList, int y]
        {
            get
            {
                var nbX = xList.Length;

                var result = new Vector<T>(nbX);

                for (var i = 0; i < nbX; i++)
                {
                    var x = xList[i];
                    result[i] = this[x, y];
                }

                return result;
            }
            set
            {
                var nbX = xList.Length;

                for (var i = 0; i < nbX; i++)
                {
                    var x = xList[i];
                    this[x, y] = value[i];
                }
            }
        }

        public Vector<T> this[int x, VectorInt yList]
        {
            get
            {
                var nbY = yList.Length;

                var result = new Vector<T>(nbY);

                for (var i = 0; i < nbY; i++)
                {
                    var y = yList[i];
                    result[i] = this[x, y];
                }

                return result;
            }
            set
            {
                var nbY = yList.Length;

                for (var i = 0; i < nbY; i++)
                {
                    var y = yList[i];
                    this[x, y] = value[i];
                }
            }
        }


        public T[] ToArray()
        {
            var flat = new T[xLength * yLength];

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    flat[y * xLength + x] = m_Grid[x, y];
                }
            }
            return flat;
        }

        public Grid<T> Clone()
        {
            var result = new Grid<T>(xLength, yLength);

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    result[x, y] = this[x, y];
                }
            }

            return result;
        }

        public Grid<T> Transposed()
        {
            var result = new Grid<T>(yLength, xLength);

            for (var x = 0; x < yLength; x++)
            {
                for (var y = 0; y < xLength; y++)
                {
                    result[x, y] = this[y, x];
                }
            }

            return result;
        }

        public void Print(Func<T, int, int, Printer> cellPrinter = null, bool reverseY = false, int cellLengthX = 3, int cellLengthY = 0, bool framed = false, int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue, int offsetX = 0, int offsetY = 0)
        {
            // Set a default cell printer if necessary
            if (cellPrinter == null)
                cellPrinter = (val, x, y) => val.ToString();

            maxX = Math.Min(maxX, xLength);
            maxY = Math.Min(maxY, yLength);

            Action<Printer> drawCell = (Printer printer) =>
            {
                var txtLength = printer.text.Length;
                var halfEmptyLength = (cellLengthX - txtLength) / 2;
                IO.PrintMultiple(" ", halfEmptyLength);
                IO.Print(printer);
                IO.PrintMultiple(" ", cellLengthX - txtLength - halfEmptyLength);
            };

            Action drawHorizontalFrame = () => {
                drawCell(new Printer(" ", ConsoleColor.DarkGray));
                IO.Print("-", ConsoleColor.DarkGray);
                for (var x = minX; x < maxX; x++)
                {
                    IO.PrintMultiple("-", cellLengthX, ConsoleColor.DarkGray);
                }
                IO.Print("-", ConsoleColor.DarkGray);
                Console.WriteLine();
            };

            // Draw the top of the frame
            if (framed)
            {
                drawCell(new Printer(" ", ConsoleColor.DarkGray));
                IO.Print(" ");

                // Print the column number
                for (var x = minX; x < maxX; x++)
                {
                    drawCell(new Printer((x + offsetX).ToString(), ConsoleColor.DarkGray));
                }

                Console.WriteLine();
                drawHorizontalFrame();
            }

            for (var y = minY; y < maxY; y++)
            {
                // Draw the left of the frame
                if (framed)
                {
                    var yValue = reverseY ? yLength - (y + offsetY) - 1 : y;
                    drawCell(new Printer((yValue + offsetY).ToString(), ConsoleColor.DarkGray));

                    IO.Print("|", ConsoleColor.DarkGray);
                }

                // Draw the content of the grid
                for (var x = minX; x < maxX; x++)
                {
                    var yValue = reverseY ? yLength - y - 1 : y;
                    var printer = cellPrinter?.Invoke(m_Grid[x, yValue], x, yValue);
                    drawCell(printer);
                }

                // Draw the right of the frame
                if (framed)
                {
                    IO.Print("|", ConsoleColor.DarkGray);
                }

                Console.WriteLine();
                IO.PrintMultiple("\n", cellLengthY);
            }

            // Draw the bottom of the frame
            if (framed)
                drawHorizontalFrame();
        }

        public Grid<K> Map<K>(Func<T, K> func)
        {
            Debug.Assert(func != null);

            var result = new Grid<K>(xLength, yLength);

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    result[x, y] = func.Invoke(this[x, y]);
                }
            }

            return result;
        }

        public static Grid<K> Combine<T1, T2, K>(Grid<T1> left, Grid<T2> right, Func<T1, T2, K> func)
        {
            Debug.Assert(left.xLength == right.xLength && left.yLength == right.yLength);

            var result = new Grid<K>(left.xLength, left.yLength);

            for (var x = 0; x < left.xLength; x++)
            {
                for (var y = 0; y < left.yLength; y++)
                {
                    result[x, y] = func.Invoke(left[x, y], right[x, y]);
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is Grid<T> other && Equals(other);
        }

        public bool Equals(Grid<T> other)
        {
            return m_Grid.Equals(other.m_Grid);
        }

        public override int GetHashCode()
        {
            return m_Grid.GetHashCode();
        }
    }

    public class GridInt : Grid<int>
    {
        public new static GridInt Constant(int xLength, int yLength, int value)
        {
            return new GridInt(Grid<int>.Constant(xLength, yLength, value));
        }

        public new static GridInt Constant(int size, int value)
        {
            return Constant(size, size, value);
        }

        public new static GridInt Diagonal(Vector<int> values)
        {
            return new GridInt(Grid<int>.Diagonal(values));
        }

        public new static GridInt Diagonal(int size, int value)
        {
            return Diagonal(VectorInt.Constant(size, value));
        }


        public GridInt(int xLength, int yLength) : base(xLength, yLength) { }

        public GridInt(int xLength, int yLength, params int[] values) : base(xLength, yLength, values) { }

        public GridInt(Grid<int> grid) : base(grid.m_Grid) { }

        public new GridInt this[VectorInt xList, VectorInt yList]
        {
            get
            {
                return new GridInt(base[xList, yList]);
            }
            set
            {
                base[xList, yList] = value;
            }
        }

        public new VectorInt this[VectorInt xList, int y]
        {
            get
            {
                return new VectorInt(base[xList, y]);
            }
            set
            {
                base[xList, y] = value;
            }
        }

        public new VectorInt this[int x, VectorInt yList]
        {
            get
            {
                return new VectorInt(base[x, yList]);
            }
            set
            {
                base[x, yList] = value;
            }
        }

        public new GridInt Clone()
        {
            return new GridInt(base.Clone());
        }

        public new GridInt Transposed()
        {
            return new GridInt(base.Transposed());
        }

        public GridInt Map(Func<int, int> func)
        {
            return new GridInt(base.Map(func));
        }

        public static GridInt Combine(GridInt left, GridInt right, Func<int, int, int> func)
        {
            return new GridInt(Grid<int>.Combine(left, right, func));
        }


        public static GridInt operator +(GridInt left, GridInt right)
        {
            return Combine(left, right, (l, r) => l + r);
        }

        public static GridInt operator -(GridInt left, GridInt right)
        {
            return Combine(left, right, (l, r) => l - r);
        }

        public static GridInt operator *(GridInt left, GridInt right)
        {
            return Combine(left, right, (l, r) => l * r);
        }

        public static GridInt operator /(GridInt left, GridInt right)
        {
            return Combine(left, right, (l, r) => l / r);
        }

        public static GridInt operator +(GridInt left, int right)
        {
            return left.Map(x => x + right);
        }

        public static GridInt operator -(GridInt left, int right)
        {
            return left.Map(x => x - right);
        }

        public static GridInt operator *(GridInt left, int right)
        {
            return left.Map(x => x * right);
        }

        public static GridInt operator /(GridInt left, int right)
        {
            return left.Map(x => x / right);
        }

        public static GridInt operator +(int left, GridInt right)
        {
            return right + left;
        }

        public static GridInt operator -(int left, GridInt right)
        {
            return right - left;
        }

        public static GridInt operator *(int left, GridInt right)
        {
            return right * left;
        }

        public static GridInt operator /(int left, GridInt right)
        {
            return right / left;
        }

        public static bool operator ==(GridInt c1, GridInt c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(GridInt c1, GridInt c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            return obj is GridInt other && Equals(other);
        }

        public bool Equals(GridInt other)
        {
            return Combine(this, other, (l, r) => l.Equals(r))
                .ToArray()
                .Aggregate(true, (acc, curr) => acc && curr);
        }

        public override int GetHashCode()
        {
            return m_Grid.GetHashCode();
        }
    }
}
