using UnityEngine;

public static partial class Functions
{
    public static Texture2D ConvertToTexture2D(this Sprite sprite, FilterMode fileterMode)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = fileterMode;
        texture.alphaIsTransparency = true;
        Color[] spriteColors = sprite.texture.GetPixels(
            (int)sprite.textureRect.x, (int)sprite.textureRect.y,
            (int)sprite.textureRect.width, (int)sprite.textureRect.height);

        texture.SetPixels(spriteColors);
        texture.Apply();

        return texture;
    }

    public static Texture2D ConvertSpriteToTexture2D(Sprite sprite, FilterMode fileterMode)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = fileterMode;
        texture.alphaIsTransparency = true;
        Color[] spriteColors = sprite.texture.GetPixels(
            (int)sprite.textureRect.x, (int)sprite.textureRect.y,
            (int)sprite.textureRect.width, (int)sprite.textureRect.height);

        texture.SetPixels(spriteColors);
        texture.Apply();

        return texture;
    }

    public static Texture2D SetPixelsAlpha(this Texture2D texture, float alphaValue)
    {
        alphaValue = Mathf.Clamp01(alphaValue);
        Color[] colors = texture.GetPixels();

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].a = alphaValue;
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}