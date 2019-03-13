using UnityEngine;

class SkyMood : Mood
{
    private MaterialPair _pair;
    
    private void Start()
    {
        var desert = Resources.Load<Material>("Materials/Skyboxes/Desert");
        var ash = Resources.Load<Material>("Materials/Skyboxes/Ash");
        _pair = new MaterialPair(desert, ash);
    }
    
    public override void SetWeight(float weight)
    {
        RenderSettings.skybox = _pair.GetBlend(weight);
    }
}