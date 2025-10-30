namespace PngDecoder.Models;
internal readonly struct PLTEData(PNGChunk palate)
{
    public readonly byte[] Palette = palate.Data.ToArray();

    // r,g,b format
    public readonly ReadOnlySpan<byte> this[int index] =>
        new(Palette, index *= 3, 3);
}
