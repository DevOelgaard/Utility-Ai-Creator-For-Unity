using UnityEngine;

namespace Tests.Editor.UnitTests
{
    public class TestHelpers
    {
        public static bool FloatEqual(float a, float b, float maxDif)
        {
            return Mathf.Abs(a - b) < maxDif;
        }

        public static bool FloatEqualTwoDecimals(float a, float b)
        {
            return FloatEqual(a, b, 0.01f);
        }
    }
}