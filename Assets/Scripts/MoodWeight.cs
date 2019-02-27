using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public abstract class MoodWeight : MonoBehaviour
{
    public GameObject cameraRig;

    private PostProcessVolume volume
    {
        get
        {
            return GetComponent<PostProcessVolume>();
        }
    }

    public void Update()
    {
        volume.weight = Mathf.Clamp(GetWeight(), 0, 1);
    }

    protected abstract float GetWeight();
}
