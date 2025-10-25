
namespace PngDecoder.Models.ColorReader;

internal class RGBAColorConverter16Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (_cnt == 0)
        {
            result[writeIndex] = inputByte;
            writeIndex++;
            _cnt++;
        }
        else
        {
            _cnt = 0;
        }
    }

    private byte _cnt;
}
