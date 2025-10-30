// Ignore Spelling: CRC

using PngDecoder.Extension;

namespace PngDecoder.Models;
internal class PNGChunk
{
    private readonly Stream _stream;

    public uint Length { get; }
    public PngChunkType Signature { get; }
    public Memory<byte> Data { get; }
    public byte[] CRC { get; }

    public PNGChunk(Stream stream)
    {
        _stream = stream;

        Span<byte> responce = stackalloc byte[4];
        stream.Read(responce);
        MemoryExtensions.Reverse(responce);
        Length = responce.ToStruct<uint>();

        stream.Read(responce);
        Signature = responce.ToStruct<PngChunkType>();

        Data = new byte[Length];
        var read = _stream.Read(Data.Span);
        if (read != Length)
            throw new IndexOutOfRangeException($"Read {read} expected Length {Length} array len {Data.Length}");

        CRC = new byte[4];
        _stream.Read(CRC);
    }
}
