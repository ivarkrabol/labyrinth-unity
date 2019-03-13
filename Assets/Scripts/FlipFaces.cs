using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FlipFaces : MonoBehaviour
{
    private void Start()
    {
        var filter = GetComponent<MeshFilter>();

        var mesh = filter.mesh;

        // Flip normals
        var normals = mesh.normals;
        for (var i = 0; i < normals.Length; i++) normals[i] = -normals[i];
        mesh.normals = normals;

        // Flip triangle vertex orders
        for (var m = 0; m < mesh.subMeshCount; m++)
        {
            var triangles = mesh.GetTriangles(m);
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }

            mesh.SetTriangles(triangles, m);
        }
    }
}