
namespace PngDecoder.Models.ColorReader;

internal class GreyScaleAndAlphaConverter8Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        result[writeIndex++] = new(pixeldata[1], pixeldata[0], pixeldata[0], pixeldata[0]);
    }
}
