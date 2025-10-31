
namespace PngDecoder.Models.ColorReader;

internal class RGBAColorConverter16Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        // ignore lower bits
        result[writeIndex++] = new(pixeldata[6], pixeldata[0], pixeldata[2], pixeldata[4]);
    }
}
