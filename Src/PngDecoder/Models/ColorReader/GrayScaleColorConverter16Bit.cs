
namespace PngDecoder.Models.ColorReader;

internal class GrayScaleColorConverter16Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (_cnt == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
            }
            result[writeIndex] = 255;
            writeIndex++;
        }
        else
        {
            _cnt = 0;
            return;
        }
        _cnt++;
    }

    private byte _cnt;
}
