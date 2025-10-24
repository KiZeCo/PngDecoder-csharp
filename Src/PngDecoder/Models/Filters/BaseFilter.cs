namespace PngDecoder.Models.Filters;
public class BaseFilter(Stream _stream, int lineWidth, byte pixelSize)
{
    private const byte FILTERTYPEFACTOR = 1;

    public Func<byte, byte> GetUnApply(byte mode) =>
        mode switch
        {
            0 => UnApply,
            1 => SubFilter,
            2 => UpFilter,
            3 => AverageFilter,
            4 => PaethFilter,
            _ => throw new NotImplementedException()
        };

    private byte SubFilter(byte current)
        => UnApply((byte)(GetLeftByte() + current));

    private byte UpFilter(byte current)
        => UnApply((byte)(GetUpByte() + current));

    private byte AverageFilter(byte current)
        => UnApply((byte)(current
            + (GetLeftByte() + GetUpByte()) / 2));

    private byte PaethFilter(byte current)
        => UnApply((byte)(current + PaethCalculate(
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

    public byte UnApply(byte current)
    {
        _stream.Seek(-1, SeekOrigin.Current);
        _stream.WriteByte(current);
        return current;
    }

    public byte GetLeftByte()
    {
        // need account the bit depth
        Span<byte> result = stackalloc byte[1];
        var modPosation = _stream.Position % lineWidth;
        if (modPosation > (pixelSize + FILTERTYPEFACTOR) || modPosation == 0)
        {
            var tempPos = _stream.Position;
            _stream.Seek(-(pixelSize + FILTERTYPEFACTOR), SeekOrigin.Current);
            _stream.Read(result);
            _stream.Seek(tempPos, SeekOrigin.Begin);
        }
        return result[0];
    }

    public byte GetUpByte()
    {
        var topIndex = _stream.Position - lineWidth - 1;
        Span<byte> result = stackalloc byte[1];
        if (topIndex >= 0)
        {
            var tempCurrentIndex = _stream.Position;
            _stream.Seek(topIndex, SeekOrigin.Begin);
            _stream.Read(result);

            _stream.Seek(tempCurrentIndex, SeekOrigin.Begin);
        }
        return result[0];
    }

    public byte GetTopLeftByte()
    {
        Span<byte> result = stackalloc byte[1];
        var topleftIndex = lineWidth + FILTERTYPEFACTOR + pixelSize;
        var modPosation = (_stream.Position - lineWidth) % lineWidth;
        if (_stream.Position > lineWidth && (modPosation > (pixelSize + FILTERTYPEFACTOR) || modPosation == 0))
        {

            var tempPos = _stream.Position;
            _stream.Seek(-(topleftIndex), SeekOrigin.Current);
            _stream.Read(result);
            _stream.Seek(tempPos, SeekOrigin.Begin);
        }
        return result[0];
    }
}
