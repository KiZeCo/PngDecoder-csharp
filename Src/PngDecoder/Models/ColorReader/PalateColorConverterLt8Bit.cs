
namespace PngDecoder.Models.ColorReader;

internal class PalateColorConverterLt8Bit(PLTEData _data, IHDRData iHdr) : IColorConverter
{
    private readonly (byte step, byte mask) bitDetails = BitDepthDetailsForPalated(iHdr.BitDepth);

    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        // less than 8 n
        for (int j = bitDetails.step; j >= 0; j -= iHdr.BitDepth)
        {
            byte mask = (byte)(bitDetails.mask << j);
            byte currentBit = (byte)((inputByte & mask) >> j);
            var colors = _data[currentBit];

            if (writeIndex < iHdr.Width * 4)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    result[writeIndex] = colors[i];
                    writeIndex++;
                }
                // for alpha
                result[writeIndex++] = 255;
            }
        }
    }

    private static (byte step, byte mask) BitDepthDetailsForPalated(byte bitDepth)
    {
        byte step = 1;
        for (byte i = 0; i < bitDepth; i++)
            step |= (byte)(1 << i);

        return ((byte)(8 - bitDepth), step);
    }
}
