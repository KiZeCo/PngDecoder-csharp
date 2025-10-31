
namespace PngDecoder.Models.ColorReader;

internal class PalateColorConverter8Bit(PLTEData _data) : IColorConverter
{
    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        var pdata = _data[pixeldata[0]];
        result[writeIndex++] = new(0xff, pdata[0], pdata[1], pdata[2]);
    }
}
