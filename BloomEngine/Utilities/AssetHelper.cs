using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.Utilities;

/// <summary>
/// Static helper class for loading assets from embedded resources.
/// </summary>
public static class AssetHelper
{
    /// <summary>
    /// Loads a sprite from an embedded resource using the specified asset path.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">
    /// The path to the embedded resource image. This must be a valid resource path within the executing assembly 
    /// (eg. "BloomEngine.Resources.Icon.png"). Make sure that the resource's Build Action is set to <strong>Embedded Resource</strong>
    /// </param>
    /// <param name="pixelsPerUnit">The number of pixels in the image that correspond to one unit in the world. Defaults to 100.</param>
    /// <returns>
    /// A <see cref="Sprite"/> object created from the embedded resource. If the resource cannot be found, the created sprite will be a 2x2 placeholder image.
    /// </returns>
    public static Sprite LoadSprite<TMarker>(string resourcePath, float pixelsPerUnit = 100f)
    {
        byte[] data = LoadResourceData<TMarker>(resourcePath);
        return CreateSpriteFromData(data, pixelsPerUnit);
    }

    /// <summary>
    /// Loads an AssetBundle from an embedded resource.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">Filename of the AssetBundle</param>
    /// <returns>The loaded AssetBundle.</returns>
    public static AssetBundle LoadAssetBundle<TMarker>(string resourcePath)
    {
        byte[] data = LoadResourceData<TMarker>(resourcePath);
        return AssetBundle.LoadFromMemory(data);
    }

    /// <summary>
    /// Retrieves the contents of an embedded resource and reads them to a byte array.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">The fully qualifiedthe embedded resource to retrieve. (eg. "BloomEngine.Assets.Icon.png")</param>
    /// <returns>A byte array containing the contents of the specified embedded resource, or an empty array if the resource is not found.</returns>
    public static byte[] LoadResourceData<TMarker>(string resourcePath)
    {
        Assembly assembly = typeof(TMarker).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(resourcePath);
        return stream is null ? [] : stream.ReadFully();
    }

    /// <summary>
    /// Creates a new Sprite from the specified image data in memory.
    /// </summary>
    /// <param name="imageData">A byte array containing image data in a supported format, such as PNG or JPEG.</param>
    /// <param name="pixelsPerUnit">The number of pixels in the image that correspond to one unit in the world. Defaults to 100.</param>
    /// <returns>A Sprite object created from the provided image data.</returns>
    public static Sprite CreateSpriteFromData(byte[] imageData, float pixelsPerUnit = 100f)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        if (!ImageConversion.LoadImage(texture, imageData, false))
            MelonLogger.Error("ImageConversion.LoadImage call failed when creating sprite from data.");

        texture.Apply(false, false);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

        return sprite;
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