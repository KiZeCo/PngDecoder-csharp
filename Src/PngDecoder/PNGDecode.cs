using PngDecoder.Exceptions;
using PngDecoder.Extension;
using PngDecoder.Models;
using PngDecoder.Models.ColorReader;
using PngDecoder.Models.Filters;
using System.Buffers;
using System.IO.Compression;

namespace PngDecoder;

public class PNGDecode
{
    private readonly Stream _fileStream;
    private readonly List<PNGChunk> _chunks = new(4);
    public IHDRData Header { get; }

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

    public byte[] DecodeImageData()
    {
        var paletteData = new PLTEData?();

        if (Header.ColorType == ColorType.Palette)
        {
            var palate = _chunks.First(a => a.Signature == PngChunkType.PLTE);
            paletteData = new PLTEData(palate);
        }
        var colorConverter = GetColorConverter(Header, paletteData);

        var writtenIndex = 0;
        var currentRow = -1;
        var result = new byte[Header.Height * Header.Width * 4];
        using var rawstream = GetFilteredRawStream();
        using var filteredMutableRawStream = new MemoryStream();
        rawstream.CopyTo(filteredMutableRawStream);
        UnfilterStream(filteredMutableRawStream, colorConverter, result, ref writtenIndex, ref currentRow);
        return result;
    }

    // TODO write own ZLib to minimize foot-print even more
    private ZLibStream GetFilteredRawStream()
    {
        var result = new MemoryStream();
        foreach (var chunk in _chunks.Where(a => a.Signature == PngChunkType.IDAT))
        {
            var data = ArrayPool<byte>.Shared.Rent((int)chunk.Length);
            try
            {
                chunk.GetData(data);
                result.Write(data, 0, (int)chunk.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
        result.Position = 0;
        return new ZLibStream(result, CompressionMode.Decompress, false);
    }

    private void UnfilterStream(Stream filteredRawData, BaseRGBColorConverter converter, byte[] result, ref int writtenIndex, ref int currentRow)
    {
        filteredRawData.Seek(0, SeekOrigin.Begin);
        Span<byte> currentByte = stackalloc byte[1];
        var writtenSection = new Span<byte>();
        var lineWidth = converter.Ihdr.GetScanLinesWidthWithPadding() + 1;
        var filterer = new BaseFilter(filteredRawData, lineWidth, converter.Ihdr.GetPixelSizeInByte());
        var unapply = filterer.GetUnApply(0);
        while (filteredRawData.Read(currentByte) != 0)
        {
            if (filteredRawData.Position == 1 || filteredRawData.Position % lineWidth == 1)
            {
                writtenIndex = 0;
                currentRow++;
                unapply = filterer.GetUnApply(currentByte[0]);
                writtenSection = new Span<byte>(result,
                    (int)(currentRow * converter.Ihdr.Width * 4),
                    (int)converter.Ihdr.Width * 4);
                continue;
            }
            //TODO: can be do prcess the number requied pixels or a full pixel.
            var compressByte = unapply(currentByte[0]);
            converter.Write(writtenSection, compressByte, ref writtenIndex);
        }
    }

    private static BaseRGBColorConverter GetColorConverter(IHDRData ihdr, PLTEData? plte) =>
        ihdr.ColorType switch
        {
            ColorType.Palette => new PalateColorConverter(plte!.Value, ihdr),
            ColorType.GreyScale => new GrayScaleColorConverter(ihdr),
            ColorType.RGB => new RGBColorConverter(ihdr),
            ColorType.GreyScaleAndAlpha => new GreyScaleAndAlphaConverter(ihdr),
            ColorType.RGBA => new RGBAColorConverter(ihdr),
            _ => throw new NotSupportedException(),
        };
}
