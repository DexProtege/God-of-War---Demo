using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{

    public static Vector3 QuadraticBezierCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 pos = 2 * (c - (2 * b) + a);
        return pos;
    }

    public static Vector3 CubicBezierCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 pos = Mathf.Pow(1 - t, 3) * a +
            3 * Mathf.Pow(1 - t, 2) * t * b +
            3 * (1 - t) * Mathf.Pow(t, 2) * c +
            Mathf.Pow(t, 3) * d;

        return pos;
    }

    public static Vector3 LinearBezierCurve(Vector3 a, Vector3 b, float t)
    {
        Vector3 pos = a + t * (b - a);
        return pos;
    }
    public static float LinearFloatCurve(float a, float b, float t)
    {
        float ans = a + t * (b - a);
        return ans;
    }
}

