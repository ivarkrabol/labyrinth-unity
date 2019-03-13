using System;
using UnityEngine;

public class TrippyTextureMood : TextureMood
{
    private const float Threshold = 0.0000001f;
    private void Update()
    {
        var t = .1 * Time.fixedTime;
        for (var y = 0; y < 200; y++)
        {
            for (var x = 0; x < 200; x++)
            {
                var level = (float)(.2 * t + Bar(1.01f * x - 100, 1.01f * y - 100, t)) % 1.0f;
                Texture.SetPixel(x, y, Color.HSVToRGB(level, 1 , 1));
            }
        }
        Texture.Apply();
    }

    private static double Foo(double x, double y, double t)
    {
        if (Math.Abs(x * y) <= Threshold)
        {
            return 0;
        }

        return Math.Sin(Math.Pow(x * x + y * y, Math.Sin(-t)) + 4 * Math.Atan(y / x) - 3 * t) / 2 + .5;
    }

    private static double Bar(double x, double y, double t)
    {
        if (Math.Abs(x * y) <= Threshold)
        {
            return 0;
        }

        return Math.Sin(Math.Pow(x * x + y * y, Math.Sin(t)) - 3 * t) / 2 + .5;
    }
}