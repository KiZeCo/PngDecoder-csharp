using PngDecoder.Models.Filters;

namespace Test;
public class BaseFilterLogics
{
    private BaseFilter filter;
    public BaseFilterLogics()
    {
        byte[] _buffer =
        {
            2, 1, 2,
            2, 1, 2,
        };
        filter = new BaseFilter(_buffer, 3, 1);
    }

    [Fact]
    public void FilterNone()
    {
        var unapply = filter.GetUnApply(0);
        Span<byte> d = [3, 5, 13, 17];
        Assert.Equal(3, unapply(0, d, null));
        Assert.Equal(5, unapply(1, d, null));
        Assert.Equal(13, unapply(2, d, null));
    }

    [Fact]
    public void LeftCheckOnFirstLine()
    {
        var unapply = filter.GetUnApply(1);
        Span<byte> d = [3, 5, 13, 17];
        Assert.Equal(3, unapply(0, d, null));
        Assert.Equal(5, unapply(1, d, null)); // no previous byte on row, pos 0 is filter type
        Assert.Equal(13, d[2]);
        Assert.Equal(13 + 5, unapply(2, d, null));
        Assert.Equal(13 + 5, d[2]);
        Assert.Equal(17, d[3]);
        Assert.Equal(17 + 13 + 5, unapply(3, d, null)); // this applies the already applied values
        Assert.Equal(17 + 13 + 5, d[3]);
    }

    [Fact]
    public void TopCheckOnFirstLine()
    {
        var unapply = filter.GetUnApply(2);
        Span<byte> d = [3, 5, 13];

        var result = unapply(2, d, null);
        Assert.Equal(13, result);
    }

    [Fact]
    public void TopCheckOnSecondLine()
    {
        var unapply = filter.GetUnApply(2);
        Span<byte> d = [3, 5, 13];

        Assert.Equal(5, d[1]);
        var result = unapply(1, d, filter.GetLine(0));
        Assert.Equal(5 + 1, result);
        Assert.Equal(5 + 1, d[1]);
    }

    [Fact]
    public void TopCheckOnSecondLineLittleInSide()
    {
        var unapply = filter.GetUnApply(2);
        Span<byte> d = [3, 5, 13];

        Assert.Equal(13, d[2]);
        var result = unapply(2, d, filter.GetLine(0));
        Assert.Equal(13 + 2, result);
        Assert.Equal(13 + 2, d[2]);
    }
}
