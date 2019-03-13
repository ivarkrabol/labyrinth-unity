using System.Collections.Generic;
using UnityEngine;

public class TextureMoodManager
{
    public const int SizeX = 200;
    public const int SizeY = 200;
    private const string TargetGameObjectsTag = "DynamicTexture";

    public static TextureMoodManager Instance
    {
        get
        {
            if (_instanceCreated) return _instance;
            _instance = new TextureMoodManager();
            _instanceCreated = true;
            return _instance;
        }
    }
    private static bool _instanceCreated;
    private static TextureMoodManager _instance;
    
    private readonly Texture2D _finalTexture;
    private readonly Dictionary<TextureMood, float> _textureWeights;

    private TextureMood _currentTextureMood;

    private TextureMoodManager()
    {
        _finalTexture = new Texture2D(SizeX, SizeY);
        _textureWeights = new Dictionary<TextureMood, float>();
        
        SetGameObjectsTexture(_finalTexture);
    }

    public void SetTextureWeight(TextureMood textureMood, float weight)
    {
        var hasChanged = false;

        if (_textureWeights.Count == 0)
        {
            _textureWeights.Add(textureMood, weight);
            _currentTextureMood = textureMood;
            hasChanged = true;
        }
        else
        {
            if (_textureWeights.ContainsKey(textureMood))
            {
                _textureWeights[textureMood] = weight;
            }
            else
            {
                _textureWeights.Add(textureMood, weight);
            }

            if (textureMood != _currentTextureMood && weight > _textureWeights[_currentTextureMood])
            {
                _currentTextureMood = textureMood;
                hasChanged = true;
            }
            else if (textureMood == _currentTextureMood && weight < _textureWeights[_currentTextureMood])
            {
                foreach (var pair in _textureWeights)
                {
                    if (pair.Key == _currentTextureMood || pair.Value <= weight) continue;
                    _currentTextureMood = pair.Key;
                    weight = pair.Value;
                    hasChanged = true;
                }
            }
        }

        if (!hasChanged) return;
        SetGameObjectsTexture(_currentTextureMood.Texture);
    }

    private void SetGameObjectsTexture(Texture toTexture)
    {
        foreach (var otherGameObject in GameObject.FindGameObjectsWithTag(TargetGameObjectsTag))
        {
            otherGameObject.GetComponent<Renderer>().material.mainTexture = toTexture;
        }
    }
}