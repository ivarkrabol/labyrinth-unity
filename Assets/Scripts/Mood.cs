using UnityEngine;

public abstract class Mood : MonoBehaviour
{
    public bool ExecuteInEditMode { get; protected set; }
    
    public abstract void SetWeight(float weight);
}