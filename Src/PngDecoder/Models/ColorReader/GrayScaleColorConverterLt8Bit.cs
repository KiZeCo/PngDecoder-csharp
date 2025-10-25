
namespace PngDecoder.Models.ColorReader;

internal class GrayScaleColorConverterLt8Bit(IHDRData iHdr) : IColorConverter
{
    private readonly (byte mask, byte bit, byte map) bitDetails = ((byte)(0xFF >> (8 - iHdr.BitDepth)), iHdr.BitDepth, (byte)((1 << iHdr.BitDepth) - 1));

    public void Write(Span<byte> result, byte inputByte, ref int writeIndex)
    {
        for (int j = 0; j < 8; j += bitDetails.bit)
        {
            byte currentBit = (byte)((byte)(((inputByte) >> (8 - bitDetails.bit - j)) & bitDetails.mask) * (255 / bitDetails.map));
            if (writeIndex < iHdr.Width * 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    result[writeIndex] = currentBit;
                    writeIndex++;
                }
                // for alpha
                result[writeIndex++] = 255;
            }
        }
    }
}
