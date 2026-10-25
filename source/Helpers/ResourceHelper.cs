using BloomEngine.Extensions;
using System.Resources;
using UnityEngine;

namespace BloomEngine.Helpers;

/// <summary>
/// Static helper class for loading various asset types from embedded resources.
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// Loads a sprite from an embedded resource. Your image file must have its Build Action set to <c>EmbeddedResource</c>.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">The fully qualified path of the embedded resource to retrieve. (e.g. "YourRootNamespace.Resources.Image.png")</param>
    /// <param name="pixelsPerUnit">The number of pixels in the image that correspond to one unit.</param>
    /// <returns>A <see cref="Sprite"/> object created from the embedded resource.</returns>
    /// <exception cref="MissingManifestResourceException">Thrown when the embedded resource could not be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the texture data is invalid or corrupt.</exception>
    public static Sprite LoadSprite<TMarker>(string resourcePath, float pixelsPerUnit = 100f)
    {
        byte[] data = LoadBytes<TMarker>(resourcePath);
        return CreateSpriteFromBytes(data, pixelsPerUnit);
    }

    /// <summary>
    /// Loads an AssetBundle from an embedded resource. Your asset bundle file must have its Build Action set to <c>EmbeddedResource</c>.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">The fully qualified path of the embedded resource to retrieve. (e.g. "YourRootNamespace.Resources.Bundle.assetbundle")</param>
    /// <returns>The <see cref="AssetBundle"/> loaded from the embedded resource.</returns>
    /// <exception cref="MissingManifestResourceException">Thrown when the embedded resource could not be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the asset bundle data is invalid or corrupt.</exception>
    public static AssetBundle LoadAssetBundle<TMarker>(string resourcePath)
    {
        byte[] data = LoadBytes<TMarker>(resourcePath);
        var bundle = AssetBundle.LoadFromMemory(data);
        
        return bundle ?? throw new InvalidOperationException("Failed to load asset bundle from memory: data is not a valid or the asset bundle format is unsupported.");
    }
    
    /// <summary>
    /// Retrieves the contents of an embedded resource and reads them to a byte array.
    /// </summary>
    /// <typeparam name="TMarker">A type which will be used to get the assembly containing the embedded resource.</typeparam>
    /// <param name="resourcePath">The fully qualified path of the embedded resource to retrieve. (e.g. "YourRootNamespace.Resources.File.txt")</param>
    /// <returns>A byte array containing the contents of the specified embedded resource.</returns>
    /// <exception cref="MissingManifestResourceException">Throws when an embedded resource could not be found at the specified path in the given assembly.</exception>
    public static byte[] LoadBytes<TMarker>(string resourcePath)
    {
        var assembly = typeof(TMarker).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if(stream is not null)
            return stream.ReadFully();
        
        throw new MissingManifestResourceException(
            $"Embedded resource '{resourcePath}' was not found in assembly '{assembly.GetName().Name}'. " +
            $"Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
    }
    
    /// <summary>
    /// Creates a new Sprite from the specified image data contained in a byte array.
    /// </summary>
    /// <param name="imageData">A byte array containing image data in a supported format, such as PNG or JPEG.</param>
    /// <param name="pixelsPerUnit">The number of pixels in the image that correspond to one unit.</param>
    /// <returns>A <see cref="Sprite"/> created from the provided image data.</returns>
    /// <exception cref="InvalidOperationException">Throws when the texture can't be loaded because the image data is invalid.</exception>
    public static Sprite CreateSpriteFromBytes(byte[] imageData, float pixelsPerUnit = 100f)
    {
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.hideFlags |= HideFlags.HideAndDontSave;

        if (!texture.LoadImage(imageData, false))
            throw new InvalidOperationException("Failed to load texture while creating sprite: image data is not a valid or uses an unsupported image format.");

        texture.Apply(false, false);
        
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        sprite.hideFlags |= HideFlags.HideAndDontSave;

        return sprite;
    }
}