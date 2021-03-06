﻿using Unity.Mathematics;

namespace AdventOfCodeTools
{
    public struct Box
    {
        public float3 min;
        public float3 length;

        public float3 max { get => min + length - 1; }

        public Box(float3 min, float3 length)
        {
            this.min = min;
            this.length = length;
        }

        public bool Contains(float3 point)
        {
            return point.x >= min.x
                && point.x <= max.x
                && point.y >= min.y
                && point.y <= max.y
                && point.z >= min.z
                && point.z <= max.z;
        }

        public bool Contains(Box other)
        {
            return Contains(math.lerp(other.min, other.max, new int3(0, 0, 0)))
                || Contains(math.lerp(other.min, other.max, new int3(0, 0, 1)))
                || Contains(math.lerp(other.min, other.max, new int3(0, 1, 0)))
                || Contains(math.lerp(other.min, other.max, new int3(0, 1, 1)))
                || Contains(math.lerp(other.min, other.max, new int3(1, 0, 0)))
                || Contains(math.lerp(other.min, other.max, new int3(1, 0, 1)))
                || Contains(math.lerp(other.min, other.max, new int3(1, 1, 0)))
                || Contains(math.lerp(other.min, other.max, new int3(1, 1, 1)));
        }

        public bool Overlaps(Box other)
        {
            return Contains(other) || other.Contains(this);
        }
    }
}
