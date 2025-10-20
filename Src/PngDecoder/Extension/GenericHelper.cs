
using System.Runtime.CompilerServices;

namespace PngDecoder.Extension;
internal static class GenericHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Equal(ReadOnlySpan<byte> data1, ReadOnlySpan<byte> data2) =>
        data1.SequenceEqual(data2);

    internal static T ToStruct<T>(this Span<byte> @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);
}
