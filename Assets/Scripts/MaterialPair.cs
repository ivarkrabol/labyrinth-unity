using UnityEngine;

public class MaterialPair
{
    private const int Steps = 24;

    private readonly Material[] _blends;
    private readonly int[] _texturePropertyNameIds;

    public MaterialPair(Material material1, Material material2)
    {
        _blends = new Material[Steps];
        _blends[0] = material1;
        _blends[Steps - 1] = material2;

        _texturePropertyNameIds = material1.GetTexturePropertyNameIDs();
        

        for (var i = 1; i < Steps - 1; i++)
        {
            _blends[i] = BlendMaterials(
                material1,
                material2,
                (float)i / (Steps - 1)
            );
        }
    }

    private Material BlendMaterials(Material baseMaterial, Material blendMaterial, float amount)
    {
        var result = new Material(baseMaterial);
        foreach (var textureNameId in _texturePropertyNameIds)
        {
            var baseTexture = baseMaterial.GetTexture(textureNameId) as Texture2D;
            if (baseTexture == null) continue;
            var blendTexture = blendMaterial.GetTexture(textureNameId) as Texture2D;
            result.SetTexture(textureNameId, BlendTextures(
                baseTexture, 
                blendTexture, 
            amount));
        }

        return result;
    }

    private static Texture BlendTextures(Texture2D baseTexture, Texture2D blendTexture, float amount)
    {
        var colors = baseTexture.GetPixels32();
        var blendColors = blendTexture.GetPixels32();
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i].r = (byte)((1f - amount) * colors[i].r + amount * blendColors[i].r);
            colors[i].g = (byte)((1f - amount) * colors[i].g + amount * blendColors[i].g);
            colors[i].b = (byte)((1f - amount) * colors[i].b + amount * blendColors[i].b);
        }
        var blend = new Texture2D(baseTexture.width, baseTexture.height);
        blend.SetPixels32(colors);
        blend.Apply();
        return blend;
    }

    public Material GetBlend(float amount)
    {
        var index = Mathf.FloorToInt(amount * Steps);
        if (index < 0) index = 0;
        if (index >= Steps) index = Steps - 1;
        return _blends[index];
    }
}