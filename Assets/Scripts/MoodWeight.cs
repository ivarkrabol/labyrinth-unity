using UnityEngine;

public abstract class MoodWeight : MonoBehaviour
{
    private bool _isCalculated;
    protected Vector3 CameraPosition;

    public void OnRenderObject()
    {
        if (_isCalculated)
        {
            _isCalculated = false;
            return;
        }
        
        CameraPosition = CameraPositionManager.CameraPosition;
        var weight = 1f;
        foreach (var moodWeight in GetComponents<MoodWeight>())
        {
            weight *= Mathf.Clamp(moodWeight.GetWeight(), 0, 1);
            moodWeight._isCalculated = true;
        }
        
        foreach (var mood in GetComponents<Mood>())
        {
            if (Application.isPlaying || mood.ExecuteInEditMode)
            {
                mood.SetWeight(weight);
            }
        }
    }

    protected abstract float GetWeight();
}
