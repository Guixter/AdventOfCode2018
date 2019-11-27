using System;
using System.Diagnostics;

namespace AdventOfCodeTools
{
    // TODO math operations
    public class VectorInt : Vector<int>
    {
        public new static VectorInt Constant(int size, int value)
        {
            var result = new VectorInt(size);

            for (var i = 0; i < size; i++)
            {
                result[i] = value;
            }

            return result;
        }

        public static VectorInt One(int size, int index)
        {
            var result = new VectorInt(size);

            for (var i = 0; i < size; i++)
            {
                if (i == index)
                    result[i] = 1;
            }

            return result;
        }

        public static VectorInt Incremental(int size, int offset = 0)
        {
            var result = new VectorInt(size);

            for (var i = 0; i < size; i++)
            {
                result[i] = i + offset;
            }

            return result;
        }


        public VectorInt(int size) : base(size) { }

        public VectorInt(int[] values) : base(values) { }


        public static implicit operator VectorInt(int[] values)
        {
            return new VectorInt(values);
        }
    }

    public class Vector<T>
    {
        public static Vector<T> Constant(int size, T value)
        {
            var result = new Vector<T>(size);

            for (var i = 0; i < size; i++)
            {
                result[i] = value;
            }

            return result;
        }


        protected Grid<T> m_Grid;
        public int Length => m_Grid.xLength;
        public VectorInt All => VectorInt.Incremental(Length);


        public Vector(int size)
        {
            m_Grid = new Grid<T>(size, 1);
        }

        public Vector(T[] values) : this(values.Length)
        {
            for (var i = 0; i < values.Length; i++)
            {
                this[i] = values[i];
            }
        }

        private Vector(Grid<T> grid)
        {
            m_Grid = grid;
        }

        public Grid<T> ToGrid(bool xOriented)
        {
            return xOriented ? m_Grid.Clone() : m_Grid.Transposed();
        }

        public T this[int i]
        {
            get => m_Grid[i, 0];
            set => m_Grid[i, 0] = value;
        }

        public Vector<T> this[VectorInt iList]
        {
            get
            {
                var result = new Vector<T>(iList.Length);
                for (var index = 0; index < iList.Length; index++)
                {
                    var i = iList[index];
                    result[index] = this[i];
                }
                return result;
            }
            set
            {
                for (var index = 0; index < iList.Length; index++)
                {
                    var i = iList[index];
                    this[i] = value[index];
                }
            }
        }

        public T[] ToArray()
        {
            return m_Grid.ToArray();
        }

        public Vector<T> Clone()
        {
            return new Vector<T>(m_Grid.Clone());
        }

        public void Print(Func<T, int, int, PrintData> cellPrinter = null)
        {
            m_Grid.Print(cellPrinter);
        }


        public static implicit operator Vector<T>(T[] values)
        {
            return new Vector<T>(values);
        }

        public static implicit operator T[](Vector<T> value)
        {
            return value.ToArray();
        }
    }
}
