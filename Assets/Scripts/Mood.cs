using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public abstract class Mood : MonoBehaviour
{   
    protected PostProcessProfile profile
    {
        get
        {
            return GetComponent<PostProcessVolume>().profile;
        }
    }
}
