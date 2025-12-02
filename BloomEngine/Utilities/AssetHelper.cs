using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.Utilities;

public static class AssetHelper
{
    private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    private static DLoadImage _iCallLoadImage;

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
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream stream = assembly.GetManifestResourceStream(assetPath);

        if (stream != null)
        {
            var data = new byte[stream.Length];
            var _ = stream.Read(data, 0, (int)stream.Length);
            LoadImage(texture, data, false);
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
    }

    private static bool LoadImage(Texture2D texture, byte[] data, bool markNonReadable)
    {
        _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
        var il2CPPArray = (Il2CppStructArray<byte>)data;

        return _iCallLoadImage.Invoke(texture.Pointer, il2CPPArray.Pointer, markNonReadable);
    }

    /// <summary>
    /// Loads an embedded AssetBundle resource from the Assets directory.
    /// </summary>
    /// <param name="bundleName">Filename of the AssetBundle</param>
    /// <returns>Loaded AssetBundle</returns>
    public static AssetBundle LoadAssetBundle(string bundleName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Assets.{bundleName}");

        if (stream is null)
            throw new ArgumentException($"Embedded resource at 'Assets/{bundleName}' not found.");

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