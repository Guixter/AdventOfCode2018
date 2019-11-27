using System;

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


        protected T[,] m_Grid;
        public int xLength { get => m_Grid.GetLength(0); }
        public int yLength { get => m_Grid.GetLength(1); }
        public VectorInt AllX => VectorInt.Incremental(xLength);
        public VectorInt AllY => VectorInt.Incremental(yLength);


        public Grid(int xLength, int yLength)
        {
            m_Grid = new T[xLength, yLength];
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
                    flat[x * yLength + y] = m_Grid[x, y];
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

        public void Print(Func<T, int, int, PrintData> cellPrinter = null, bool reverseY = false, int cellLengthX = 3, int cellLengthY = 0, bool framed = false, int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue, int offsetX = 0, int offsetY = 0)
        {
            // Set a default cell printer if necessary
            if (cellPrinter == null)
                cellPrinter = (val, x, y) => val.ToString();

            maxX = Math.Min(maxX, xLength);
            maxY = Math.Min(maxY, yLength);

            Action<PrintData> drawCell = (PrintData printer) =>
            {
                var txtLength = printer.text.Length;
                var halfEmptyLength = (cellLengthX - txtLength) / 2;
                IO.PrintMultiple(' ', halfEmptyLength);
                IO.Print(printer.text, printer.color, printer.background);
                IO.PrintMultiple(' ', cellLengthX - txtLength - halfEmptyLength);
            };

            Action drawHorizontalFrame = () => {
                drawCell(new PrintData(" ", ConsoleColor.DarkGray));
                IO.Print('-', ConsoleColor.DarkGray);
                for (var x = minX; x < maxX; x++)
                {
                    IO.PrintMultiple('-', cellLengthX, ConsoleColor.DarkGray);
                }
                IO.Print('-', ConsoleColor.DarkGray);
                Console.WriteLine();
            };

            // Draw the top of the frame
            if (framed)
            {
                drawCell(new PrintData(" ", ConsoleColor.DarkGray));
                IO.Print(' ');

                // Print the column number
                for (var x = minX; x < maxX; x++)
                {
                    drawCell(new PrintData((x + offsetX).ToString(), ConsoleColor.DarkGray));
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
                    drawCell(new PrintData((yValue + offsetY).ToString(), ConsoleColor.DarkGray));

                    IO.Print('|', ConsoleColor.DarkGray);
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
                    IO.Print('|', ConsoleColor.DarkGray);
                }

                Console.WriteLine();
                IO.PrintMultiple('\n', cellLengthY);
            }

            // Draw the bottom of the frame
            if (framed)
                drawHorizontalFrame();
        }



        public static implicit operator Grid<T>(T value)
        {
            var result = new Grid<T>(1, 1);
            result[0, 0] = value;
            return result;
        }
    }

    public class PrintData {
        public string text = "";
        public ConsoleColor color = ConsoleColor.White;
        public ConsoleColor background = ConsoleColor.Black;

        public PrintData(string text = "", ConsoleColor color = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            this.text = text;
            this.color = color;
            this.background = background;
        }

        public static implicit operator PrintData(string value)
        {
            return new PrintData() {
                text = value
            };
        }

        public static implicit operator PrintData(char value)
        {
            return new PrintData()
            {
                text = value.ToString()
            };
        }
    }

    // TODO math operations
    public class GridInt : Grid<int>
    {
        public new static GridInt Constant(int xLength, int yLength, int value)
        {
            var result = new GridInt(xLength, yLength);

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    result[x, y] = value;
                }
            }

            return result;
        }

        public new static GridInt Constant(int size, int value)
        {
            return Constant(size, size, value);
        }

        public new static GridInt Diagonal(Vector<int> values)
        {
            var size = values.Length;
            var result = new GridInt(size, size);

            for (var i = 0; i < size; i++)
            {
                result[i, i] = values[i];
            }

            return result;
        }

        public new static GridInt Diagonal(int size, int value)
        {
            return Diagonal(VectorInt.Constant(size, value));
        }


        public GridInt(int xLength, int yLength) : base (xLength, yLength) { }

        public new VectorInt this[VectorInt xList, int y]
        {
            get
            {
                var nbX = xList.Length;

                var result = new VectorInt(nbX);

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

        public new VectorInt this[int x, VectorInt yList]
        {
            get
            {
                var nbY = yList.Length;

                var result = new VectorInt(nbY);

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
    }
}
