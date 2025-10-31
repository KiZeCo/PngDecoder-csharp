
namespace PngDecoder.Models.ColorReader;

internal class RGBColorConverter16Bit() : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        // ignore lower bits
        result[writeIndex++] = new(0xff, pixeldata[0], pixeldata[2], pixeldata[4]);
    }
}
