namespace BloomEngine.Extensions;

/// <summary>
/// Provides extensions for Stream types.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Fully reads a Stream into a byte array.
    /// </summary>
    /// <param name="input">Input stream to copy from.</param>
    /// <returns>Output byte array containing the contents of the Stream.</returns>
    public static byte[] ReadFully(this Stream input)
    {
        using var stream = new MemoryStream();
        input.CopyTo(stream);
        return stream.ToArray();
    }
}
