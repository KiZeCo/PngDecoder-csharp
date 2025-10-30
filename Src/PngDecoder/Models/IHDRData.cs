namespace PngDecoder.Models;
public readonly struct IHDRData
{
    public uint Width { get; }
    public uint Height { get; }
    public byte BitDepth { get; }
    public ColorType ColorType { get; }
    public byte CompressionMethod { get; }
    public byte FilterMethod { get; }
    public byte InterlaceMethod { get; }

    internal IHDRData(PNGChunk headerChunk)
    {
        if (headerChunk.Length != 13)
            throw new ArgumentException("Invalid Header");

        Span<byte> response = headerChunk.Data.Span;

        var temp = response[..4];
        temp.Reverse();
        Width = BitConverter.ToUInt32(temp);

        temp = response.Slice(4, 4);
        temp.Reverse();
        Height = BitConverter.ToUInt32(temp);

        BitDepth = response[8];
        ColorType = (ColorType)response[9];
        CompressionMethod = response[10];
        FilterMethod = response[11];
        InterlaceMethod = response[12];
    }

    public readonly int GetScanLinesWidthWithPadding()
    {
        var length = Width * BitDepth * BytePerPixels;
        var count = (int)(length / 8);
        var extra = length % 8;

        if (extra == 0)
            return count;
        return ++count;
    }

    public readonly decimal GetScanLineWidthWithoutPadding()
    {
        decimal length = Width * BitDepth * BytePerPixels;
        return length / 8m;
    }

    private readonly uint BytePerPixels => ColorType switch
    {
        ColorType.GreyScale => 1,
        ColorType.RGB => 3,
        ColorType.Palette => 1,
        ColorType.GreyScaleAndAlpha => 2,
        ColorType.RGBA => 4,
        _ => throw new Exception(),
    };

    public readonly byte PixelSizeInByte => ColorType switch
    {
        ColorType.GreyScale => (byte)Math.Round(1d * BitDepth / 8, MidpointRounding.ToPositiveInfinity),
        ColorType.Palette => 1,
        ColorType.GreyScaleAndAlpha => (byte)(2 * BitDepth / 8),
        ColorType.RGB => (byte)(3 * BitDepth / 8),
        ColorType.RGBA => (byte)(4 * BitDepth / 8),
        _ => throw new Exception()
    };
}
