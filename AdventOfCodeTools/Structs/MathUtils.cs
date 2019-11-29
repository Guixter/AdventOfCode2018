using System.Linq;
using Unity.Mathematics;

namespace AdventOfCodeTools
{
    public static partial class MathUtils
    {

#region Array conversion

        public static bool[] ToArray(this bool3 values)
        {
            return new bool[] {
                values.x,
                values.y,
                values.z,
            };
        }

        public static bool[] ToArray(this bool2 values)
        {
            return new bool[] {
                values.x,
                values.y,
            };
        }

        public static int[] ToArray(this int3 values)
        {
            return new int[] {
                values.x,
                values.y,
                values.z,
            };
        }

        public static int[] ToArray(this int2 values)
        {
            return new int[] {
                values.x,
                values.y,
            };
        }

        public static float[] ToArray(this float3 values)
        {
            return new float[] {
                values.x,
                values.y,
                values.z,
            };
        }

        public static float[] ToArray(this float2 values)
        {
            return new float[] {
                values.x,
                values.y,
            };
        }

        #endregion

#region boolean operations
        public static bool All(this bool3 values)
        {
            return values.ToArray().All(x => x);
        }

        public static bool All(this bool2 values)
        {
            return values.ToArray().All(x => x);
        }

        public static bool Any(this bool3 values)
        {
            return values.ToArray().Any(x => x);
        }

        public static bool Any(this bool2 values)
        {
            return values.ToArray().Any(x => x);
        }

        public static bool None(this bool3 values)
        {
            return !values.Any();
        }

        public static bool None(this bool2 values)
        {
            return !values.Any();
        }
#endregion

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
