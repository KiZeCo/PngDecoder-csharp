using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
using System.Diagnostics;
using System.IO.Compression;

namespace PngDecoder;

public class PNGDecode
{
    private readonly Stream _fileStream;
    private readonly List<PNGChunk> _chunks = new(4);
    public IHDRData Header { get; }
    private PLTEData PlteData => new(_chunks.First(a => a.Signature == PngChunkType.PLTE));

    private static ReadOnlySpan<byte> headerSignature =>
        [0x89, (byte)'P', (byte)'N', (byte)'G', (byte)'\r', (byte)'\n', 0x1a, (byte)'\n'];

    public PNGDecode(Stream fileStream)
    {
        _fileStream = fileStream;
        Span<byte> signature = stackalloc byte[headerSignature.Length];
        _fileStream.Read(signature);
        if (!GenericHelper.Equal(signature, headerSignature))
            throw new SignatureException(signature.ToArray());

        var gotHeader = false;
        while (_fileStream.Position < _fileStream.Length)
        {
            var chunk = new PNGChunk(_fileStream);
            if (!gotHeader && chunk.Signature == PngChunkType.IHDR)
            {
                Header = new (chunk);
                gotHeader = true;
            }
            _chunks.Add(chunk);
            if (chunk.Signature == PngChunkType.IEND)
                break;
        }

        //check essentials
        // IEND, IDAT, IHDR
    }

    public uint Height => Header.Height;
    public uint Width => Header.Width;

    public Memory<byte> DecodeImageData()
    {
        var colorConverter = GetColorConverter();

        var result = new byte[Header.Height * Header.Width * 4];
        var mutableRawData = GetFilteredRawStream();
        UnfilterStream(mutableRawData, colorConverter, result);
        return result;
    }

    // TODO write own ZLib to minimize foot-print even more
    private Memory<byte> GetFilteredRawStream()
    {
        using var result = new MemoryStream();
        foreach (var chunk in _chunks.Where(a => a.Signature == PngChunkType.IDAT))
        {
            result.Write(chunk.Data.Span);
        }
        result.Position = 0;
        using var src = new ZLibStream(result, CompressionMode.Decompress, false);
        using var mutableRawStream = new MemoryStream((int)result.Length);
        src.CopyTo(mutableRawStream);
        mutableRawStream.Position = 0;
        return new Memory<byte>(mutableRawStream.GetBuffer(), 0, (int)mutableRawStream.Length);
    }

    private void UnfilterStream(Memory<byte> mutableRawData, IColorConverter converter, Span<byte> result)
    {
        var writtenSection = new Span<byte>();
        var lineWidth = Header.GetScanLinesWidthWithPadding() + 1;
        var filterer = new BaseFilter(mutableRawData, lineWidth, Header.PixelSizeInByte);
        var unapply = filterer.GetUnApply(0);
        var writtenIndex = 0;
        var currentRow = -1;
        var currentByte = -1;
        // TODO work on lines
        var lineno = 0;
        while (lineno < Height)
        {
            var line = filterer.GetLine(lineno++);
            if (line.Length != lineWidth)
            {
                throw new IndexOutOfRangeException($"{line.Length} expected {lineWidth}");
            }
        }
        while ((currentByte = filterer.ReadByte()) != -1)
        {
            if (filterer.Position % lineWidth == 1)
            {
                writtenIndex = 0;
                currentRow++;
                unapply = filterer.GetUnApply(currentByte);
                writtenSection = result.Slice(
                    (int)(currentRow * Header.Width * 4),
                    (int)Header.Width * 4);
                continue;
            }
            //TODO: can be do prcess the number requied pixels or a full pixel.
            var compressByte = unapply((byte)currentByte);
            converter.Write(writtenSection, compressByte, ref writtenIndex);
        }
    }

    private IColorConverter GetColorConverter() =>
        Header.ColorType switch
        {
            ColorType.Palette => Header.BitDepth == 8 ? new PalateColorConverter8Bit(PlteData) : new PalateColorConverterLt8Bit(PlteData, Header),
            ColorType.GreyScale => Header.BitDepth == 8 ? new GrayScaleColorConverter8Bit() : (Header.BitDepth < 8 ? new GrayScaleColorConverterLt8Bit(Header) : new GrayScaleColorConverter16Bit()),
            ColorType.RGB => Header.BitDepth == 8 ? new RGBColorConverter8Bit() : new RGBColorConverter16Bit(),
            ColorType.GreyScaleAndAlpha => Header.BitDepth == 8 ? new GreyScaleAndAlphaConverter8Bit() : new GreyScaleAndAlphaConverter16Bit(),
            ColorType.RGBA => Header.BitDepth == 8 ? new RGBAColorConverter8Bit() : new RGBAColorConverter16Bit(),
            _ => throw new NotSupportedException(),
        };
}
