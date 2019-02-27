using UnityEngine;

public class BloomEffect : MonoBehaviour
{
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
}
