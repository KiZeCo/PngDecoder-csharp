using PngDecoder.Models.Filters;

namespace Test;
public class BaseFilterForSixteenBitImage
{
    private MemoryStream _stream;
    public BaseFilterForSixteenBitImage()
    {
        _stream = new MemoryStream(20);

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
        var filter = new BaseFilter(new Memory<byte>(_stream.GetBuffer(), 0, (int)_stream.Length), 10, 3);
        var unapply = filter.GetUnApply(1); // SubFilter
        var line = filter.GetLine(0);
        var pos = line[4];
        Assert.Equal(8, pos);
        var leftApplied = unapply(4, line, null);

        Assert.Equal(pos * 2, leftApplied);
    }
}
