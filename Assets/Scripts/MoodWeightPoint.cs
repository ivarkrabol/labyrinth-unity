using UnityEngine;

[ExecuteInEditMode]
public class MoodWeightPoint : MoodWeight
{
    public Vector3 point;
    public float innerRadius = 5;
    public float outerRadius = 100;

    protected override float GetWeight()
    {
        var distance = Vector3.Distance(CameraPosition, point);
        return 1 - (distance - innerRadius) / (outerRadius - innerRadius);
    }
}
