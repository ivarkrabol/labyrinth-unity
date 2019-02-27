using UnityEngine;

public class MoodWeightPoint : MoodWeight
{
    public Vector3 point;
    public float innerRadius = 5;
    public float outerRadius = 100;

    override protected float GetWeight()
    {
        float distance = Vector3.Distance(cameraRig.transform.position, point);
        return 1 - (distance - innerRadius) / (outerRadius - innerRadius);
    }
}
