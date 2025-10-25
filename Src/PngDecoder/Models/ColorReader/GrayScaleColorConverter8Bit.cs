
namespace PngDecoder.Models.ColorReader;

internal class GrayScaleColorConverter8Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        for (int i = 0; i < 3; i++)
        {
            result[writeIndex] = inputByte;
            writeIndex++;
        }

        result[writeIndex] = 255;
        writeIndex++;
    }
}
