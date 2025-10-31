
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PngDecoder.Models;

[DebuggerDisplay("{ArgbValueStr}")]
public readonly struct Argb(uint argb)
{
    private const int ShiftA = 24;
    private const int ShiftR = 16;
    private const int ShiftG = 8;
    private const int ShiftB = 0;

    public readonly uint Value => argb;

    public byte A => unchecked((byte)(Value >> ShiftA));
    public byte R => unchecked((byte)(Value >> ShiftR));
    public byte G => unchecked((byte)(Value >> ShiftG));
    public byte B => unchecked((byte)(Value >> ShiftB));

    public Argb(byte alpha, byte red, byte green, byte blue)
        : this(
        (uint)alpha << ShiftA |
        (uint)red << ShiftR |
        (uint)green << ShiftG |
        (uint)blue << ShiftB)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMaxRgb(out byte min, out byte max, byte r, byte g, byte b)
    {
        if (r > g)
        {
            max = r;
            min = g;
        }
        else
        {
            max = g;
            min = r;
        }
        if (b > max)
        {
            max = b;
        }
        else if (b < min)
        {
            min = b;
        }
    }

    public float Brightness
    {
        get
        {
            MinMaxRgb(out byte min, out byte max, R, G, B);

            return (max + min) / (byte.MaxValue * 2f);
        }
    }

    private string ArgbValueStr => $"{{ARGB = (0x{A:x2}, 0x{R:x2}, 0x{G:x2}, 0x{B:x2})}}";

    public int ToArgb() => unchecked((int)Value);

    public static bool operator ==(Argb left, Argb right) => left.Value == right.Value;
    public static bool operator !=(Argb left, Argb right) => left.Value != right.Value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Argb other && Equals(other);

    public bool Equals(Argb other) => this == other;

    public override int GetHashCode() => ToArgb();

    public override string ToString() => $"{A:x2}{R:x2}{G:x2}{B:x2}";
}