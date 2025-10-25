
namespace PngDecoder.Models.ColorReader;

internal interface IColorConverter
{
    void Write(Span<byte> result, byte inputByte, ref int writeIndex);
}
