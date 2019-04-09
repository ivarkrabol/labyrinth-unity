using System;
using UnityEditor;
using UnityEngine;

public class Trippy : MonoBehaviour
{
    public static TrippyMaterial Instance;
    private void Update()
    {
        Instance?.Update();
    }
}

public class TrippyMaterial
{    
    private const float Threshold = 0.0000001f;

    public Material Material { get; }
    
    private readonly Texture2D _texture;

    public TrippyMaterial()
    {
        _texture = new Texture2D(100, 100);
        Material = new Material(Shader.Find("Standard")) {mainTexture = _texture};
        Trippy.Instance = this;
    }

    public void Update()
    {
        var t = .04 * Time.fixedTime + .2;
        for (var y = 0; y < 100; y++)
        {
            for (var x = 0; x < 100; x++)
            {
                var level = (float) (.5 * t + Foo(1.01f * x - 50, 1.01f * y - 50, t)) % 1.0f;
                _texture.SetPixel(x, y, Color.HSVToRGB(level, 1, 1));
            }
        }

        _texture.Apply();
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