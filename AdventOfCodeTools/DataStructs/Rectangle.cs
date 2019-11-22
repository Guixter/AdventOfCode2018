namespace AdventOfCodeTools
{
    public struct Rectangle<T>
    {
        public float xMin;
        public float yMin;
        public float xLength;
        public float yLength;
        public T meta;

        public float xMax
        {
            get => xMin + xLength - 1;
        }


        public float yMax
        {
            get => yMin + yLength - 1;
        }

        public bool Contains(float x, float y)
        {
            return x >= xMin
                && x <= xMax
                && y >= yMin
                && y <= yMax;
        }

        public bool Contains<K>(Point2<K> point)
        {
            return Contains(point.x, point.y);
        }

        public bool Contains(Rectangle<T> other)
        {
            return Contains(other.xMin, other.yMin)
                || Contains(other.xMin, other.yMax)
                || Contains(other.xMax, other.yMin)
                || Contains(other.xMax, other.yMax);
        }

        public bool Overlaps(Rectangle<T> other)
        {
            return Contains(other) || other.Contains(this);
        }
    }
}
