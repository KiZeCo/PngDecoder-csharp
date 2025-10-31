
namespace PngDecoder.Models.ColorReader;

internal interface IColorConverter
{
    void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex);
}
