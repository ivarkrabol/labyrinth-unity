using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Vector3 = UnityEngine.Vector3;

[ExecuteInEditMode]
public class CameraPositionManager : MonoBehaviour
{
    public static Vector3 CameraPosition { get; private set; }

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        Debug.Assert(_mainCamera != null, nameof(_mainCamera) + " != null");
        _mainCamera.fieldOfView = 100f;
    }

    public void Update()
    {
        CameraPosition = _mainCamera.transform.position;
    }
}