using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MoodWeightInOut : MoodWeight
{
    public Vector3 entrance;
    public Vector3 exit;
    public Vector3 bottomLeft;
    public float size = 42.0f;
    public float blendRatio = 0.5f;

    protected override float GetWeight()
    {
        var distEntrance = Vector3.Distance(CameraPosition, entrance) / (14f * blendRatio);
        var distExit = Vector3.Distance(CameraPosition, exit) / (14f * blendRatio);
        return CameraPosition.x >= bottomLeft.x && CameraPosition.x <= bottomLeft.x + size &&
               CameraPosition.z >= bottomLeft.z && CameraPosition.z <= bottomLeft.z + size
            ? .5f * (1f + Mathf.Clamp(Math.Min(distExit, distEntrance), 0f, 1f))
            : .5f * (1f - Mathf.Clamp(Math.Min(distExit, distEntrance), 0f, 1f));
    }
}
