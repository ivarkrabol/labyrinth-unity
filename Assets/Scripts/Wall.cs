using UnityEngine;

public class Wall : MonoBehaviour
{
    public void Start()
    {
        Material material = GetComponent<Renderer>().material;
        Color color = material.color;
        color.b = 1f;
        material.color = color;
    }
}
