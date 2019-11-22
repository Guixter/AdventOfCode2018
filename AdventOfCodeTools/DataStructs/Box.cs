namespace AdventOfCodeTools
{
    public struct Box<T>
    {
        public float xMin;
        public float yMin;
        public float zMin;
        public float xLength;
        public float yLength;
        public float zLength;
        public T meta;

        public float xMax
        {
            get => xMin + xLength - 1;
        }

        public float yMax
        {
            get => yMin + yLength - 1;
        }

        public float zMax
        {
            get => zMin + zLength - 1;
        }

        public bool Contains(float x, float y, float z)
        {
            return x >= xMin
                && x <= xMax
                && y >= yMin
                && y <= yMax
                && z >= zMin
                && z <= zMax;
        }

        public bool Contains<K>(Point3<K> point)
        {
            return Contains(point.x, point.y, point.z);
        }

        public bool Contains(Box<T> other)
        {
            return Contains(other.xMin, other.yMin, other.zMin)
                || Contains(other.xMin, other.yMax, other.zMin)
                || Contains(other.xMax, other.yMin, other.zMin)
                || Contains(other.xMax, other.yMax, other.zMin)
                || Contains(other.xMin, other.yMin, other.zMax)
                || Contains(other.xMin, other.yMax, other.zMax)
                || Contains(other.xMax, other.yMin, other.zMax)
                || Contains(other.xMax, other.yMax, other.zMax);
        }

        public bool Overlaps(Box<T> other)
        {
            return Contains(other) || other.Contains(this);
        }
    }
}
