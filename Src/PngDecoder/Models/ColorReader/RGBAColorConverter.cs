
namespace PngDecoder.Models.ColorReader;
internal class RGBAColorConverter(IHDRData ihdr) : BaseRGBColorConverter(ihdr)
{
    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (Ihdr.BitDepth == 8)
        {
            result[writeIndex] = inputByte;
            writeIndex++;
        }
        else // 16
        {
            if (_cnt == 0)
            {
                result[writeIndex] = inputByte;
                writeIndex++;
                _cnt++;
            }
            else
            {
                _cnt = 0;
                return;
            }
        }
    }

    private byte _cnt;
}
