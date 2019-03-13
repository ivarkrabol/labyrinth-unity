using UnityEngine;

public abstract class TextureMood : Mood
{
    public Texture2D Texture { get; private set; }

    private void Start()
    {
        Texture = new Texture2D(TextureMoodManager.SizeX, TextureMoodManager.SizeY);
    }

    public override void SetWeight(float weight)
    {
        TextureMoodManager.Instance.SetTextureWeight(this, weight);
    }
}