using Unity.Mathematics;

namespace AdventOfCodeTools
{
    public static partial class MathUtils
    {
        public static float3 Merge(float3 left, float3 right, int3 merger)
        {
            return left * merger + right * (1 - merger);
        }

        public static float2 Merge(float2 left, float2 right, int2 merger)
        {
            return left * merger + right * (1 - merger);
        }

        public static bool All(this bool3 values)
        {
            return values.x && values.y && values.z;
        }

        public static bool All(this bool2 values)
        {
            return values.x && values.y;
        }

        public static float ManhattanDistance(float3 left, float3 right)
        {
            var delta = left - right;
            return math.abs(delta.x) + math.abs(delta.y) + math.abs(delta.z);
        }

        public static float ManhattanDistance(float2 left, float2 right)
        {
            var delta = left - right;
            return math.abs(delta.x) + math.abs(delta.y);
        }
    }
}
