
namespace PngDecoder.Models.ColorReader;

internal class RGBColorConverter8Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (writeIndex % 4 == 3)
        {
            result[writeIndex] = 255;
            writeIndex++;
        }
        result[writeIndex] = inputByte;
        writeIndex++;

        // fill in alpha for last pixel on line
        if (writeIndex == result.Length - 1)
        {
            result[writeIndex] = 255;
        }
    }
}
