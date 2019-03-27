using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[ExecuteInEditMode]
public class CameraPositionManager : MonoBehaviour
{
    public static Vector3 CameraPosition { get; private set; }

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        _mainCamera.fieldOfView = 100f;
    }

    public void OnRenderObject()
    {
        CameraPosition = !Application.isPlaying 
            ? SceneView.lastActiveSceneView.pivot 
            : _mainCamera.transform.position;
    }
}