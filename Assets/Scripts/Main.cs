using System;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    private Texture2D _texture;
    private void Start()
    {
        var walls = GameObject.FindGameObjectsWithTag("DynamicTexture");
        var material = walls[0].GetComponent<Renderer>().material;
        var colors = Enumerable.Repeat(Color.red, 200 * 200).ToArray();
        _texture = new Texture2D(200, 200);
        _texture.SetPixels(colors);
        _texture.Apply();
        material.mainTexture = _texture;
        foreach (var wall in walls)
        {
            wall.GetComponent<Renderer>().material = material;
        }
    }

    private void Update()
    {
        var t = .1 * Time.fixedTime;
        for (var y = 0; y < 200; y++)
        {
            for (var x = 0; x < 200; x++)
            {
                var level = (float)Bar(x, y, t);
                _texture.SetPixel(x, y, Color.HSVToRGB(level, 1 , 1));
            }
        }
        _texture.Apply();
    }

    private static double Foo(double x, double y, double t)
    {
        if (Math.Abs(x * y) <= .00001)
        {
            return 0;
        }

        return (.2 * t + Math.Sin(Math.Pow(x * x + y * y, Math.Sin(-t)) + 4 * Math.Atan(y / x) - 3 * t) / 2 + .5) % 1;
    }

    private static double Bar(double x, double y, double t)
    {
        if (Math.Abs(x * y) <= .00001)
        {
            return 0;
        }

        return (.2 * t + Math.Sin(Math.Pow(x * x + y * y, Math.Sin(t)) - 3 * t) / 2 + .5) % 1;
    }
}