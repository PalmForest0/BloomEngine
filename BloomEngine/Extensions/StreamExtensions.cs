namespace BloomEngine.Extensions;

/// <summary>
/// Provides extension methods for certain Stream types.
/// </summary>
public static class StreamExtensions
{
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
