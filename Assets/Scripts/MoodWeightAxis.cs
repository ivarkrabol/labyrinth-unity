using UnityEngine;

[ExecuteInEditMode]
public class MoodWeightAxis : MoodWeight
{
    public Axis axis;
    public float valueMin = -100;
    public float valueMax = 100;

    override protected float GetWeight()
    {
        float position = Vector3.Dot(cameraRig.transform.position, axis == Axis.X ? Vector3.right : Vector3.forward);
        return (position - valueMin) / (valueMax - valueMin);
    }
}

public enum Axis
{
    X, Z
}
