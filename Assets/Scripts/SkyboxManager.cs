using UnityEngine;

public class SkyboxManager
{
    private MaterialPair[] _pairs;

    public SkyboxManager(Material[] skyboxCycle)
    {
        _pairs = new MaterialPair[skyboxCycle.Length];
        _pairs[0] = new MaterialPair(skyboxCycle[skyboxCycle.Length - 1], skyboxCycle[0]);
        for (var i = 1; i < skyboxCycle.Length; i++)
        {
            _pairs[i] = new MaterialPair(skyboxCycle[i - 1], skyboxCycle[i]);
        }
    }
}
