using System;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material[] skyboxCycle = new Material[8];
    public float blendRatio = 0.2f;
    public int blendSteps = 20;

    private MaterialPair[] _pairs;

    private static readonly Vector3[] Transitions = {
        new Vector3(102, 0, 81), 
        new Vector3(81, 0, 102), 
        new Vector3(39, 0, 102), 
        new Vector3(18, 0, 81), 
        new Vector3(18, 0, 39), 
        new Vector3(39, 0, 18), 
        new Vector3(81, 0, 18), 
        new Vector3(102, 0, 39), 
    };

    private void Start()
    {
        if (skyboxCycle.Length == 0)
        {
            enabled = false;
            return;
        }

        _pairs = new MaterialPair[skyboxCycle.Length];
        for (var i = 0; i < skyboxCycle.Length; i++)
        {
            _pairs[i] = new MaterialPair(
                skyboxCycle[i],
                skyboxCycle[(i + 1) % skyboxCycle.Length],
                blendSteps
            );
        }
    }

    private void Update()
    {
        var pos = CameraPositionManager.CameraPosition;
        pos.y = 0;
        var cyclePosition = GetCyclePosition(pos);
        RenderSettings.skybox = GetMaterial(cyclePosition);
    }

    private float GetCyclePosition(Vector3 pos)
    {
        if (pos.x < 39)
        {
            if (pos.z < 39) return GetCyclePositionForIndex(pos, 5);
            if (pos.z < 81) return GetCyclePositionForIndex(pos, 4);
            return GetCyclePositionForIndex(pos, 3);
        }
        if (pos.x < 81)
        {
            if (pos.z < 39) return GetCyclePositionForIndex(pos, 6);
            return GetCyclePositionForIndex(pos, 2);
        }
        if (pos.z < 39) return GetCyclePositionForIndex(pos, 7);
        if (pos.z < 81)  return GetCyclePositionForIndex(pos, 0);
        return GetCyclePositionForIndex(pos, 1);
    }
    
    private float GetCyclePositionForIndex(Vector3 pos, int nextIndex)
    {
        var prevIndex = (nextIndex + 7) % 8;
        var distPrev = Vector3.Distance(pos, Transitions[prevIndex]) / (14f * blendRatio);
        var distNext = Vector3.Distance(pos, Transitions[nextIndex]) / (14f * blendRatio);
        return distPrev < distNext
            ? prevIndex + .5f * (1f + Mathf.Clamp(distPrev, 0f, 1f))
            : nextIndex + .5f * (1f - Mathf.Clamp(distNext, 0f, 1f));
    }

    private Material GetMaterial(float cyclePosition)
    {
        while (cyclePosition < 0) cyclePosition += _pairs.Length;
        while (cyclePosition >= _pairs.Length) cyclePosition -= _pairs.Length;
        var index = (int) cyclePosition;
        if (Math.Abs(blendRatio) < 0.001) return _pairs[index].GetBlend(cyclePosition - index > 0.5f ? 1 : 0);
        var blendAmount = (cyclePosition - index - 0.5f * (1 - blendRatio)) / blendRatio;
        return _pairs[index].GetBlend(blendAmount);
    }
}