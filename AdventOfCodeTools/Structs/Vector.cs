using System;
using System.Diagnostics;

namespace AdventOfCodeTools
{
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


        internal Grid<T> m_Grid;
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

        internal Vector(Grid<T> values)
        {
            m_Grid = values;
        }

        public static implicit operator Vector<T>(T[] values)
        {
            return new Vector<T>(values);
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


        public Grid<T> ToGrid(bool xOriented)
        {
            return xOriented ? m_Grid.Clone() : m_Grid.Transposed();
        }

        public T[] ToArray()
        {
            return m_Grid.ToArray();
        }

        public Vector<T> Clone()
        {
            var result = new Vector<T>(Length);

            for (var i = 0; i < Length; i++)
            {
                result[i] = this[i];
            }

            return result;
        }


        public void Print(Func<T, int, int, Printer> cellPrinter = null)
        {
            m_Grid.Print(cellPrinter);
        }

        public Vector<K> Map<K>(Func<T, K> func)
        {
            return new Vector<K>(m_Grid.Map(func));
        }

        public static Vector<K> Combine<T1, T2, K>(Vector<T1> left, Vector<T2> right, Func<T1, T2, K> func)
        {
            return new Vector<K>(Grid<T>.Combine(left.m_Grid, right.m_Grid, func));
        }
    }

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

        internal VectorInt(Vector<int> vector) : base(vector.m_Grid) { }

        public static implicit operator VectorInt(int[] values)
        {
            return new VectorInt(values);
        }


        public new VectorInt this[VectorInt iList]
        {
            get
            {
                return new VectorInt(base[iList]);
            }
            set
            {
                base[iList] = value;
            }
        }

        public new VectorInt Clone()
        {
            return new VectorInt(base.Clone());
        }


        public VectorInt Map(Func<int, int> func)
        {
            return new VectorInt(base.Map(func));
        }

        public static VectorInt Combine(VectorInt left, VectorInt right, Func<int, int, int> func)
        {
            return new VectorInt(Vector<int>.Combine(left, right, func));
        }


        public static VectorInt operator +(VectorInt left, VectorInt right)
        {
            return Combine(left, right, (l, r) => l + r);
        }

        public static VectorInt operator -(VectorInt left, VectorInt right)
        {
            return Combine(left, right, (l, r) => l - r);
        }

        public static VectorInt operator *(VectorInt left, VectorInt right)
        {
            return Combine(left, right, (l, r) => l * r);
        }

        public static VectorInt operator /(VectorInt left, VectorInt right)
        {
            return Combine(left, right, (l, r) => l / r);
        }

        public static VectorInt operator +(VectorInt left, int right)
        {
            return left.Map(x => x + right);
        }

        public static VectorInt operator -(VectorInt left, int right)
        {
            return left.Map(x => x - right);
        }

        public static VectorInt operator *(VectorInt left, int right)
        {
            return left.Map(x => x * right);
        }

        public static VectorInt operator /(VectorInt left, int right)
        {
            return left.Map(x => x / right);
        }

        public static VectorInt operator +(int left, VectorInt right)
        {
            return right + left;
        }

        public static VectorInt operator -(int left, VectorInt right)
        {
            return right - left;
        }

        public static VectorInt operator *(int left, VectorInt right)
        {
            return right * left;
        }

        public static VectorInt operator /(int left, VectorInt right)
        {
            return right / left;
        }
    }
}
