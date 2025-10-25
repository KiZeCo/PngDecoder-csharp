
namespace PngDecoder.Models.ColorReader;

internal class RGBColorConverter16Bit() : IColorConverter
{
    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (_canRead)
        {
            result[writeIndex++] = inputByte;
            if (writeIndex % 4 == 3)
            {
                result[writeIndex++] = 255;
            }
        }
        _canRead = !_canRead;
    }
    private bool _canRead = true;
}
