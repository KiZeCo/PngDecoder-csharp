using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
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

    public Memory<Argb> DecodeImageData()
    {
        var colorConverter = GetColorConverter();

        var result = new Memory<Argb>(new Argb[Header.Height * Header.Width]);
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

    private void UnfilterStream(Memory<byte> mutableRawData, IColorConverter converter, Memory<Argb> result)
    {
        Span<Argb> writtenSection;
        var lineWidth = Header.GetScanLinesWidthWithPadding() + 1;
        var pxSize = Header.PixelSizeInByte;
        var filterer = new BaseFilter(mutableRawData, lineWidth, pxSize);
        var currentRow = 0;
        Span<byte> priorLine = null;
        Span<byte> pixelbytes = stackalloc byte[pxSize];
        while (currentRow < Height)
        {
            writtenSection = result.Slice(
                (int)(currentRow * Header.Width),
                (int)Header.Width).Span;
            var line = filterer.GetLine(currentRow++);
            var unapply = filterer.GetUnApply(line[0]);

            var writtenIndex = 0;
            var pos = 1; // ignore first command byte
            var pxpos = 0;
            while (pos < lineWidth)
            {
                pixelbytes[pxpos++] = unapply(pos++, line, priorLine);
                if (pxpos == pxSize)
                {
                    converter.Write(writtenSection, pixelbytes, ref writtenIndex);
                    pxpos = 0;
                }
            }
            if (pxpos != 0)
            {
                throw new InvalidOperationException($"pxpos was {pxpos}");
            }
            priorLine = line;
        }
    }

    private IColorConverter GetColorConverter() =>
        Header.ColorType switch
        {
            ColorType.Palette => Header.BitDepth == 8 ? new PalateColorConverter8Bit(PlteData) : new PalateColorConverterLt8Bit(PlteData, Header),
            ColorType.Greyscale => Header.BitDepth == 8 ? new GrayScaleColorConverter8Bit() : (Header.BitDepth < 8 ? new GrayScaleColorConverterLt8Bit(Header) : new GrayScaleColorConverter16Bit()),
            ColorType.Color => Header.BitDepth == 8 ? new RGBColorConverter8Bit() : new RGBColorConverter16Bit(),
            ColorType.AlphaUsed => Header.BitDepth == 8 ? new GreyScaleAndAlphaConverter8Bit() : new GreyScaleAndAlphaConverter16Bit(),
            ColorType.RGBA => Header.BitDepth == 8 ? new RGBAColorConverter8Bit() : new RGBAColorConverter16Bit(),
            _ => throw new NotSupportedException(),
        };
}
