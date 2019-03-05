using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public abstract class MoodWeight : MonoBehaviour
{
    protected Vector3 CameraPosition;
    private bool _usingEditorCamera;

    private PostProcessVolume Volume => GetComponent<PostProcessVolume>();

    public void OnRenderObject()
    {
        if (!_usingEditorCamera && !Application.isPlaying)
        {
            CameraPosition = SceneView.lastActiveSceneView.pivot;
            _usingEditorCamera = true;
        } else if (_usingEditorCamera && Camera.main != null)
        {
            CameraPosition = Camera.main.transform.position;
            _usingEditorCamera = false;
        }
    }

    public void Update()
    {
        Volume.weight = Mathf.Clamp(GetWeight(), 0, 1);
    }

    protected abstract float GetWeight();
}
