using Unity.Mathematics;

namespace AdventOfCodeTools
{
    public struct Rectangle
    {
        public float2 min;
        public float2 length;

        public float2 max { get => min + length - 1; }

        public Rectangle(float2 min, float2 length)
        {
            this.min = min;
            this.length = length;
        }

        public bool Contains(float2 point)
        {
            return point.x >= min.x
                && point.x <= max.x
                && point.y >= min.y
                && point.y <= max.y;
        }

        public bool Contains(Rectangle other)
        {
            return Contains(MathUtils.Merge(other.min, other.max, new int2(0, 0)))
                || Contains(MathUtils.Merge(other.min, other.max, new int2(0, 1)))
                || Contains(MathUtils.Merge(other.min, other.max, new int2(1, 0)))
                || Contains(MathUtils.Merge(other.min, other.max, new int2(1, 1)));
        }

        public bool Overlaps(Rectangle other)
        {
            return Contains(other) || other.Contains(this);
        }
    }
}
