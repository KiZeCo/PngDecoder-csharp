
namespace PngDecoder.Models.ColorReader;

internal class PalateColorConverterLt8Bit(PLTEData _data, IHDRData iHdr) : IColorConverter
{
    private readonly (byte step, byte mask) bitDetails = BitDepthDetailsForPalated(iHdr.BitDepth);

    public void Write(Span<Argb> result, Span<byte> pixeldata, ref int writeIndex)
    {
        var inputByte = pixeldata[0];
        // less than 8 n
        for (int j = bitDetails.step; j >= 0; j -= iHdr.BitDepth)
        {
            byte mask = (byte)(bitDetails.mask << j);
            byte currentBit = (byte)((inputByte & mask) >> j);
            var colors = _data[currentBit];

            // overflow is ignoerd?
            if (writeIndex < iHdr.Width)
            {
                result[writeIndex++] = new(0xff, colors[0], colors[1], colors[2]);
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
