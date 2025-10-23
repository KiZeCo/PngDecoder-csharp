// Ignore Spelling: ihdr

namespace PngDecoder.Models.ColorReader;
internal class RGBColorConverter(IHDRData ihdr) : BaseRGBColorConverter(ihdr)
{
    public override void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        if (Ihdr.BitDepth == 8)
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
        // 16
        else
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
    }
    private bool _canRead = true;
}
