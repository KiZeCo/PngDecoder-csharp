
namespace PngDecoder.Models.ColorReader;

internal class GreyScaleAndAlphaConverter8Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (writeIndex % 2 == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
            }
        }
        else
        {
            result[writeIndex] = inputByte;
            writeIndex++;
        }
    }
}
