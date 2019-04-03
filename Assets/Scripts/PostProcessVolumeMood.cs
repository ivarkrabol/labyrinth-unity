using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PostProcessVolumeMood : Mood
{
    public PostProcessVolumeMood()
    {
        ExecuteInEditMode = true;
    }

    private PostProcessVolume Volume => GetComponent<PostProcessVolume>();
    public override void SetWeight(float weight)
    {
        Volume.weight = weight;
    }
}