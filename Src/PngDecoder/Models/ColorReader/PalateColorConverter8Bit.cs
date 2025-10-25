
namespace PngDecoder.Models.ColorReader;

internal class PalateColorConverter8Bit(PLTEData _data) : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (writeIndex % 4 == 3)
        {
            result[writeIndex] = 255;
            writeIndex++;
        }
        result[writeIndex] = _data[inputByte][0];
        writeIndex++;
        result[writeIndex] = _data[inputByte][1];
        writeIndex++;
        result[writeIndex] = _data[inputByte][2];
        writeIndex++;
    }
}
