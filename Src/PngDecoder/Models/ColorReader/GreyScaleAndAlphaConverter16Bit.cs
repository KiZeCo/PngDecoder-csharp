
namespace PngDecoder.Models.ColorReader;

internal class GreyScaleAndAlphaConverter16Bit() : IColorConverter
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
        }
        else if (_cnt == 2)
        {
            result[writeIndex] = inputByte;
            writeIndex++;
        }
        else if (_cnt == 3)
        {
            _cnt = 0;
            return;
        }
        _cnt++;
    }

    private byte _cnt = 0;
}
