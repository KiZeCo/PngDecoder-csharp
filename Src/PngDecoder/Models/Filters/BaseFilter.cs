
using System.Runtime.CompilerServices;

namespace PngDecoder.Models.Filters;
public class BaseFilter(Memory<byte> buffer, int lineWidth, byte pixelSize)
{
    public readonly int Length = buffer.Length;
    private readonly Memory<byte> Buffer = buffer;
    public Span<byte> GetLine(int lineno) => Buffer.Slice(lineno * lineWidth, lineWidth).Span;
    
    public delegate byte UnApplyDelegate(int pos, Span<byte> line, Span<byte> priorLine);

    public UnApplyDelegate GetUnApply(int mode) =>
        mode switch
        {
            0 => None,
            1 => SubFilter,
            2 => UpFilter,
            3 => AverageFilter,
            4 => PaethFilter,
            _ => throw new NotImplementedException($"Unknown mode {mode}"),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte None(int pos, Span<byte> line, Span<byte> priorLine) => line[pos];

    private byte SubFilter(int pos, Span<byte> line, Span<byte> priorLine)
        => UnApply(pos, line, GetLeftByte(pos, line));

    private byte UpFilter(int pos, Span<byte> line, Span<byte> priorLine)
        => UnApply(pos, line, GetUpByte(pos, priorLine));

    private byte AverageFilter(int pos, Span<byte> line, Span<byte> priorLine)
        => UnApply(pos, line, (GetLeftByte(pos, line) + GetUpByte(pos, priorLine)) / 2);

    private byte PaethFilter(int pos, Span<byte> line, Span<byte> priorLine)
        => UnApply(pos, line, PaethPredictor(
            GetLeftByte(pos, line),
            GetUpByte(pos, priorLine),
            priorLine.Length == 0 ? (byte)0 : GetLeftByte(pos, priorLine)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte PaethPredictor(byte left, byte top, byte upperLeft)
    {
        var p = left + top - upperLeft;
        var pa = Math.Abs(p - left);
        var pb = Math.Abs(p - top);
        var pc = Math.Abs(p - upperLeft);
        if (pa <= pb && pa <= pc)
        {
            return left;
        }
        else if (pb <= pc)
        {
            return top;
        }

        return upperLeft;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte UnApply(int pos, Span<byte> line, int apply)
    {
        var v = line[pos];
        apply = (byte)(v + apply);
        return v == apply
            ? (byte)apply
            : (line[pos] = (byte)apply);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetLeftByte(int pos, Span<byte> line)
    {
        // need account the bit depth
        // first byte on line is filter type
        if (pos <= pixelSize)
        {
            return 0;
        }
        return line[pos - pixelSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte GetUpByte(int pos, Span<byte> priorLine)
        => priorLine.Length == 0 ? (byte)0 : priorLine[pos];
}
