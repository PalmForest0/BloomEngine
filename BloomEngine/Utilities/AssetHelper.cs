using System.Reflection;
using UnityEngine;

namespace BloomEngine.Utilities;

/// <summary>
/// Static helper class for loading assets from embedded resources.
/// </summary>
public static class AssetHelper
{
    /// <summary>
    /// Loads a sprite from an embedded resource stream using the specified asset path.
    /// </summary>
    /// <remarks>The method attempts to load the specified asset as an embedded resource from the executing
    /// assembly. If the resource is not found, a placeholder texture is used to create the sprite. The sprite's pivot
    /// is set to the center of the texture, and the pixels-per-unit value is derived from the texture's
    /// width.</remarks>
    /// <param name="assetPath">The path to the embedded resource containing the sprite data. This must be a valid resource path within the
    /// executing assembly.</param>
    /// <returns>A <see cref="Sprite"/> object created from the embedded resource. The sprite is generated with default pivot and
    /// pixels-per-unit values. If the resource cannot be found, the sprite will be created from a 1x1 placeholder
    /// texture.</returns>
    public static Sprite LoadSprite(string assetPath)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(assetPath);

        if (stream is null)
            throw new ArgumentException($"Embedded image resource not found: {assetPath}", nameof(assetPath));

        byte[] data = stream.ReadFully();

        if (!ImageConversion.LoadImage(texture, data, false))
            throw new Exception("ImageConversion.LoadImage call failed");

        texture.Apply(false, false);

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }


    /// <summary>
    /// Loads an embedded AssetBundle resource from the Assets directory.
    /// </summary>
    /// <param name="bundlePath">Filename of the AssetBundle</param>
    /// <returns>Loaded AssetBundle</returns>
    public static AssetBundle LoadAssetBundle(string bundlePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(bundlePath);

        if (stream is null)
            throw new ArgumentException($"Embedded resource at 'Assets/{bundlePath}' not found.", nameof(bundlePath));

        return AssetBundle.LoadFromMemory(stream.ReadFully());
    }

    /// <summary>
    /// Fully reads a stream into a byte array.
    /// </summary>
    /// <param name="input">Input stream</param>
    /// <returns>Output byte array</returns>
    public static byte[] ReadFully(this Stream input)
    {
        using MemoryStream stream = new MemoryStream();
        input.CopyTo(stream);
        return stream.ToArray();
    }
}