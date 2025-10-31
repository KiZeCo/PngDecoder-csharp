
namespace PngDecoder.Models.ColorReader;

internal class RGBAColorConverter8Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        result[writeIndex++] = new(pixeldata[3], pixeldata[0], pixeldata[1], pixeldata[2]);
    }
}
