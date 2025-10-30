using PngDecoder;

namespace Test;

public class UnitTest1
{
    // http://www.schaik.com/pngsuite

    private static DirectoryInfo GetBasePath()
    {
        var projPath = Environment.GetEnvironmentVariable("NCrunch.OriginalProjectPath") ?? Path.Combine(Directory.GetCurrentDirectory(), "dummy.proj");
        return new DirectoryInfo(Path.GetDirectoryName(projPath) ?? throw new NullReferenceException("projPath"));
    }

    private static IEnumerable<FileInfo> GetFileList()
    {
        var dir = GetBasePath();
        Assert.True(dir.Exists);

        return dir.EnumerateFiles("*.png", SearchOption.AllDirectories);
    }

    private const string TestFiles = nameof(TestFiles);
    private static readonly string TestFilesPath = Path.Combine(GetBasePath().FullName, TestFiles);
    public static IEnumerable<object[]> GetTestFilenames() 
        => GetFileList().Select(f => new object[] { f.FullName.Replace(TestFilesPath, "").Trim(Path.DirectorySeparatorChar) });

    [Theory]
    [MemberData(nameof(GetTestFilenames))]
    public void PngFileTest(string filename)
    {
        var filepath = Path.Combine(TestFilesPath, filename);
        using var fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            Console.WriteLine($"File: {filepath}");
            var file = Path.GetFileName(filepath);
            if (file.StartsWith('x'))
            {
                Console.WriteLine("Skipping file with expected error");
                return;
            }
            var png = new PNGDecode(fs);
            Console.WriteLine($"Width: {png.Width}, Height: {png.Height}");
            var header = png.Header;
            Console.WriteLine($"ColorType: {header.ColorType}");
            Console.WriteLine($"BitDepth: {header.BitDepth}");
            Console.WriteLine($"CompressionMethod: {header.CompressionMethod}");
            Console.WriteLine($"FilterMethod: {header.FilterMethod}");
            Console.WriteLine($"InterlaceMethod: {header.InterlaceMethod}");

            if (header.InterlaceMethod != 0)
            {
                Console.WriteLine("Interleaced not supported for now, skipping");
                return;
            }

            var c = png.DecodeImageData().ToArray();

            // assert
            Assert.NotEmpty(c);
            Console.WriteLine($"Pixels size: {c.Length}");
            foreach (var lchunk in c.Chunk((int)png.Width * 4))
            {
                Console.WriteLine(string.Join(", ", lchunk
                    .Chunk(4).Select(bs => string.Join("", bs.Select(b => $"{b:x2}")))));
            }

            if (header.BitDepth == 2 && header.ColorType == PngDecoder.Models.ColorType.GreyScale) // basi0g02, basn0g02
            {
                Assert.Fail();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(filepath);
            Console.WriteLine(ex.ToString());
            throw;
        }
        fs.Close();
    }
}
