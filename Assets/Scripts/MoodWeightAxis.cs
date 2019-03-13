using UnityEngine;

[ExecuteInEditMode]
public class MoodWeightAxis : MoodWeight
{
    public Axis axis;
    public float valueMin = -100;
    public float valueMax = 100;
    public bool reverse = false;

    protected override float GetWeight()
    {
        var position = Vector3.Dot(CameraPosition, axis.GetUnitVector());
        var weight = (position - valueMin) / (valueMax - valueMin);
        if (reverse) return 1 - weight;
        return weight;
    }
}

public enum Axis
{
    X, Y, Z
}

internal static class AxisMethods {
    public static Vector3 GetUnitVector(this Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return Vector3.right;
            case Axis.Y:
                return Vector3.up;
            case Axis.Z:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }
}
