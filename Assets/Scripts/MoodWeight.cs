using UnityEngine;

[RequireComponent(typeof(Mood))]
[DisallowMultipleComponent]
public abstract class MoodWeight : MonoBehaviour
{
    protected Vector3 CameraPosition;

    public void OnRenderObject()
    {
        CameraPosition = CameraPositionManager.CameraPosition;
        foreach (var mood in GetComponents<Mood>())
        {
            if (Application.isPlaying || mood.ExecuteInEditMode)
            {
                mood.SetWeight(Mathf.Clamp(GetWeight(), 0, 1));
            }
        }
    }

    protected abstract float GetWeight();
}
