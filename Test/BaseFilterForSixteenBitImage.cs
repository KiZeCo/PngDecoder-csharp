using PngDecoder.Models.Filters;

namespace Test;
public class BaseFilterForSixteenBitImage
{

    private MemoryStream _stream;
    public BaseFilterForSixteenBitImage()
    {
        _stream = new MemoryStream(20);
        _stream.Position = 0;

        _stream.WriteByte(99); // Filter              
            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

            _stream.WriteByte(8); // R
            _stream.WriteByte(9); // G
            _stream.WriteByte(10); // B

        _stream.WriteByte(99); // Filter    
            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B

            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B

            _stream.WriteByte(18); // R
            _stream.WriteByte(19); // G
            _stream.WriteByte(20); // B
    }

    [Fact]
    public void Get3PartLeft()
    {
        var filter = new BaseFilter(new Memory<byte>(_stream.GetBuffer(), 0, (int)_stream.Length), 9, 3);
        filter.Position = 4;
        var pos  = filter.ReadByte();
        var left = filter.GetLeftByte();

        Assert.Equal(pos, left);
    }
}
