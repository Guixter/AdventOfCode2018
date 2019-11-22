using System;

namespace AdventOfCodeTools
{
    public struct Rectangle<T>
    {
        public float left;
        public float top;
        public float width;
        public float height;
        public T meta;

        public float right
        {
            get => left + width - 1;
        }


        public float bottom
        {
            get => top + height - 1;
        }

        public bool Contains(float i, float j)
        {
            return i >= left
                && i <= right
                && j >= top
                && j <= bottom;
        }

        public bool Contains(Rectangle<T> other)
        {
            return Contains(other.left, other.top)
                || Contains(other.left, other.bottom)
                || Contains(other.right, other.top)
                || Contains(other.right, other.bottom);
        }

        public bool Overlaps(Rectangle<T> other)
        {
            return Contains(other) || other.Contains(this);
        }
    }
}
