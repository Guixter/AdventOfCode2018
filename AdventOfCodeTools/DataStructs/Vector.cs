using Unity.Mathematics;

namespace AdventOfCodeTools
{
    // TODO math operations on vector
    public struct VectorInt
    {
        public static VectorInt Constant(int size, int value)
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

        public static VectorInt Incremental(int size)
        {
            var result = new VectorInt(size);

            for (var i = 0; i < size; i++)
            {
                result[i] = i + 1;
            }

            return result;
        }


        private int[] m_Array;

        public int Length => m_Array.Length;

        public VectorInt(int size)
        {
            m_Array = new int[size];
        }

        public int[] ToArray()
        {
            return m_Array;
        }

        public int this[int x]
        {
            get => m_Array[x];
            set => m_Array[x] = value;
        }

        public static implicit operator VectorInt(int value)
        {
            var result = new VectorInt(1);
            result[0] = value;
            return result;
        }

        public static implicit operator VectorInt(int[] value)
        {
            var result = new VectorInt(value.Length);

            for (var i = 0; i < value.Length; i++)
            {
                result[i] = value[i];
            }

            return result;
        }
    }
}
