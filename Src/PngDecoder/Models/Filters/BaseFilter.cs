
namespace PngDecoder.Models.Filters;
public class BaseFilter(Memory<byte> buffer, int lineWidth, byte pixelSize)
{
    private const byte FILTERTYPEFACTOR = 1;

    private readonly int Length = buffer.Length;
    private readonly Memory<byte> Buffer = buffer;
    public int ReadByte()
        => Length <= Position ? -1 : GetByte(Position++);

    public int Position;

    // TODO allocate span per row
    public byte GetByte(long pos) => Buffer.Slice((int)pos, 1).Span[0];

    public Func<byte, byte> GetUnApply(int mode) =>
        mode switch
        {
            0 => None,
            1 => SubFilter,
            2 => UpFilter,
            3 => AverageFilter,
            4 => PaethFilter,
            _ => throw new NotImplementedException($"Unknown mode {mode}"),
        };

    private byte None(byte current) => current;

    private byte SubFilter(byte current)
        => UnApply(current, (byte)(GetLeftByte() + current));

    private byte UpFilter(byte current)
        => UnApply(current, (byte)(GetUpByte() + current));

    private byte AverageFilter(byte current)
        => UnApply(current, (byte)(current
            + (GetLeftByte() + GetUpByte()) / 2));

    private byte PaethFilter(byte current)
        => UnApply(current, (byte)(current + PaethCalculate(
            GetLeftByte(),
            GetUpByte(),
            GetTopLeftByte())));

    private static byte PaethCalculate(byte left, byte top, byte upperLeft)
    {
        var p = left + top - upperLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - top);
        var pc = Math.Abs(p - upperLeft);
        if (pa <= pb && pa <= pc)
            return left;
        else if (pb <= pc)
            return top;
        else
            return upperLeft;
    }

    public byte UnApply(byte current, byte apply)
    {
        if (current != apply)
        {
            Buffer.Slice(Position - 1, 1).Span[0] = apply;
        }
        return apply;
    }

    public byte GetLeftByte()
    {
        // need account the bit depth
        var tempPos = Position;
        var modPosation = tempPos % lineWidth;
        if (modPosation != 0 && modPosation <= (pixelSize + FILTERTYPEFACTOR))
        {
            return 0;
        }
        return GetByte(tempPos - (pixelSize + FILTERTYPEFACTOR));
    }

    public byte GetUpByte()
    {
        var topIndex = Position - lineWidth - 1;
        if (topIndex < 0)
        {
            return 0;
        }
        return GetByte(topIndex);
    }

    public byte GetTopLeftByte()
    {
        var tempPos = Position;
        var modPosation = (tempPos - lineWidth) % lineWidth;
        if (tempPos <= lineWidth ||
            modPosation != 0 && modPosation <= (pixelSize + FILTERTYPEFACTOR))
        {
            return 0;
        }
        var topleftIndex = lineWidth + FILTERTYPEFACTOR + pixelSize;
        return GetByte(tempPos - topleftIndex);
    }
}
