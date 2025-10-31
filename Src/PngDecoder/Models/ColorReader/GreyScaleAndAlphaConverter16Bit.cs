
namespace PngDecoder.Models.ColorReader;

internal class GreyScaleAndAlphaConverter16Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        // ignore lower bits
        result[writeIndex++] = new(pixeldata[2], pixeldata[0], pixeldata[0], pixeldata[0]);
    }
}
