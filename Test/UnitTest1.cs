using PngDecoder;
using System.Diagnostics;

namespace Test;

public class UnitTest1
{
    private List<FileInfo> _list;
    public UnitTest1()
    {
        var projPath = Environment.GetEnvironmentVariable("NCrunch.OriginalProjectPath") ?? Path.Combine(Directory.GetCurrentDirectory(), "dummy.proj");
        var dir = new DirectoryInfo(Path.GetDirectoryName(projPath));
        Assert.True(dir.Exists);

        _list = [.. dir.EnumerateFiles("*.png", SearchOption.AllDirectories)];
    }

    [Fact]
    public void TotalFiles()
    {
        Assert.Equal(262, _list.Count);
    }

    [Fact]
    public void FirstBatch()
    {
        // arrange
        // act
        foreach (var file in _list)
        {
            using var fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
            try
            {
                var pngDecode = new PNGDecode(fs);
                var c = pngDecode.DecodeImageData();

                // assert
                Assert.NotEmpty(c);
                Console.WriteLine($"File: {file.FullName}");
                Console.WriteLine($"Height: {pngDecode.Height}, Width: {pngDecode.Width}");
                var header = pngDecode.Header;
                Console.WriteLine($"ColorType: {header.ColorType}");
                Console.WriteLine($"BitDepth: {header.BitDepth}");
                Console.WriteLine($"CompressionMethod: {header.CompressionMethod}");
                Console.WriteLine($"FilterMethod: {header.FilterMethod}");
                Console.WriteLine($"InterlaceMethod: {header.InterlaceMethod}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(file.FullName);
                Console.WriteLine(ex.ToString());
                throw;
            }
            fs.Close();
        }
        Debug.Assert(0 != _list.Count);
    }
}
