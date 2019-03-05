using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public abstract class MoodWeight : MonoBehaviour
{
    protected Vector3 CameraPosition;

    private PostProcessVolume Volume => GetComponent<PostProcessVolume>();

    public void OnRenderObject()
    {
        CameraPosition = CameraPositionManager.CameraPosition;
        Volume.weight = Mathf.Clamp(GetWeight(), 0, 1);
    }

    protected abstract float GetWeight();
}
