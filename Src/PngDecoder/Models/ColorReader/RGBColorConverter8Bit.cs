
namespace PngDecoder.Models.ColorReader;

internal class RGBColorConverter8Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        result[writeIndex++] = new(0xff, pixeldata[0], pixeldata[1], pixeldata[2]);
    }
}
