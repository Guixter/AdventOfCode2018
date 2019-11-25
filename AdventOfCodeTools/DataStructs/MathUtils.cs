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
    }
}
