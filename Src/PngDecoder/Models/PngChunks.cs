// Ignore Spelling: CRC

using PngDecoder.Extension;

namespace PngDecoder.Models;
internal struct PNGChunk
{
    private readonly Stream _stream;

    public uint Length { get; }
    public PngChunkType Signature { get; }
    private long PosData { get; }
    private long PosCRC { get; }

    public PNGChunk(Stream stream)
    {
        _stream = stream;

        Span<byte> responce = stackalloc byte[4];
        stream.Read(responce);
        MemoryExtensions.Reverse(responce);
        Length = responce.ToStruct<uint>();

        stream.Read(responce);
        Signature = responce.ToStruct<PngChunkType>();

        PosData = stream.Position;
        _stream.Seek(Length, SeekOrigin.Current);

        PosCRC = stream.Position;
        _stream.Seek(4, SeekOrigin.Current);
    }

    public int GetData(Span<byte> result)
    {
        var oldPosation = _stream.Position;
        _stream.Seek(PosData, SeekOrigin.Begin);
        var read = _stream.Read(result);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
        return read;
    }

    public Span<byte> GetCRC()
    {
        Span<byte> result = new byte[4];
        var oldPosation = _stream.Position;
        _stream.Seek(PosCRC, SeekOrigin.Begin);
        _stream.Read(result);
        _stream.Seek(oldPosation, SeekOrigin.Begin);
        return result;
    }
}
