
namespace PngDecoder.Models.ColorReader;

internal class RGBAColorConverter8Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        result[writeIndex] = inputByte;
        writeIndex++;
    }
}
