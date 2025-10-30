using PngDecoder.Models.Filters;

namespace Test;
public class BaseFilterLogics
{
    private BaseFilter filter;
    public BaseFilterLogics()
    {
        var _stream = new MemoryStream();
        _stream.WriteByte(2); _stream.WriteByte(1); _stream.WriteByte(2);
        _stream.WriteByte(2); _stream.WriteByte(1); _stream.WriteByte(2);
        filter = new BaseFilter(new Memory<byte>(_stream.GetBuffer(), 0, (int)_stream.Length), 3, 1);
    }

    [Fact]
    public void LeftCheckOnFirstLine()
    {
        filter.Position = 1;
        _ = filter.ReadByte();
        Assert.Equal(2, filter.Position);

        var result = filter.GetLeftByte();
        Assert.Equal(0, result);
        Assert.Equal(2, filter.Position);
    }

    [Fact]
    public void LeftCheckOnSecondLine()
    {
        filter.Position = 5;
        _ = filter.ReadByte();
        Assert.Equal(6, filter.Position);

        var result = filter.GetLeftByte();
        Assert.Equal(1, result);
    }

    [Fact]
    public void LeftCheckOnFirstLineLittleInSide()
    {
        filter.Position = 2;
        _ = filter.ReadByte();

        var result = filter.GetLeftByte();
        Assert.Equal(1, result);
    }

    [Fact]
    public void LeftCheckOnSecondLineLittleInSide()
    {
        filter.Position = 5;
        _ = filter.ReadByte();

        var result = filter.GetLeftByte();
        Assert.Equal(1, result);
    }

    [Fact]
    public void TopCheckOnFirstLine()
    {
        filter.Position = 1;
        _ = filter.ReadByte();

        var result = filter.GetUpByte();
        Assert.Equal(0, result);
    }

    [Fact]
    public void TopCheckOnFirstLineLittleInSide()
    {
        filter.Position = 2;
        _ = filter.ReadByte();

        var result = filter.GetUpByte();
        Assert.Equal(0, result);
    }


    [Fact]
    public void TopCheckOnSecondLine()
    {
        filter.Position = 4;

        var result = filter.GetUpByte();
        Assert.Equal(2, result);
    }

    [Fact]
    public void TopCheckOnSecondLineLittleInSide()
    {
        filter.Position = 5;
        _ = filter.ReadByte();

        var result = filter.GetUpByte();
        Assert.Equal(2, result);
    }

    [Fact]
    public void TopLeftCheckOnSecondLine()
    {
        filter.Position = 5;

        var response = filter.GetTopLeftByte();
        Assert.Equal(0, response);
    }

    [Fact]
    public void TopLeftCheckSecondLineLittleInSide()
    {
        filter.Position = 6;

        var response = filter.GetTopLeftByte();
        Assert.Equal(1, response);
    }

    [Fact]
    public void TopLeftCheckFirstLine()
    {
        filter.Position = 2;

        var response = filter.GetTopLeftByte();
        Assert.Equal(0, response);
    }

    [Fact]
    public void TopLeftCheckFirstLineLittleInSide()
    {
        filter.Position = 3;

        var response = filter.GetTopLeftByte();
        Assert.Equal(0, response);
    }

    [Fact]
    public void AfterLeftCheckPositionCheck()
    {
        filter.Position = 0;

        var pos1 = filter.ReadByte();
        Assert.Equal(2, pos1);
        var pos2 = filter.ReadByte();
        Assert.Equal(1, pos2);
        var leftPos1 = filter.GetLeftByte();
        Assert.Equal(0, leftPos1);
        var pos3 = filter.ReadByte();
        Assert.Equal(2, pos3);
        var leftPos2 = filter.GetLeftByte();
        Assert.Equal(pos2, leftPos2);

        var pos4 = filter.ReadByte();
        Assert.Equal(2, pos4);
        var pos5 = filter.ReadByte();
        Assert.Equal(1, pos5);
        var leftPos3 = filter.GetLeftByte();
        Assert.Equal(0, leftPos3);
        var pos6 = filter.ReadByte();
        Assert.Equal(2, pos6);
        var leftPos4 = filter.GetLeftByte();
        Assert.Equal(pos5, leftPos4);
    }

    [Fact]
    public void AfterTopCheckPositionCheck()
    {
        filter.Position = 0;

        var pos1 = filter.ReadByte();
        var pos2 = filter.ReadByte();
        var topPos1 = filter.GetUpByte();
        var pos3 = filter.ReadByte();
        var topPos2 = filter.GetUpByte();

        var pos4 = filter.ReadByte();
        var pos5 = filter.ReadByte();
        var topPos3 = filter.GetUpByte();
        var pos6 = filter.ReadByte();
        var topPos4 = filter.GetUpByte();

        var check = pos1 == 2
            && pos2 == 1
            && pos3 == 2
            && pos4 == 2
            && pos5 == 1
            && pos6 == 2
            && topPos1 == 0
            && topPos2 == 0
            && topPos3 == 1
            && topPos4 == 2;
        Assert.True(check);
    }

    [Fact]
    public void AfterTopLeftCheckPositionCheck()
    {
        filter.Position = 0;

        var pos1 = filter.ReadByte();
        var pos2 = filter.ReadByte();
        var topLeftPos1 = filter.GetTopLeftByte();
        var pos3 = filter.ReadByte();
        var topLeftPos2 = filter.GetTopLeftByte();

        var pos4 = filter.ReadByte();
        var pos5 = filter.ReadByte();
        var topLeftPos3 = filter.GetLeftByte();
        var pos6 = filter.ReadByte();
        var topLeftPos4 = filter.GetLeftByte();

        var check = pos1 == 2
            && pos2 == 1
            && pos3 == 2
            && pos4 == 2
            && pos5 == 1
            && pos6 == 2
            && topLeftPos1 == 0
            && topLeftPos2 == 0
            && topLeftPos3 == 0
            && topLeftPos4 == 1;
        Assert.True(check);
    }

    [Fact]
    public void CheckWrite()
    {
        filter.Position = 0;
        var response = filter.ReadByte();
        Assert.Equal(2, response);

        filter.UnApply(0, 10);
        var response2 = filter.ReadByte();
        Assert.Equal(1, response2);
        filter.Position = 0;
        var newResponse = filter.ReadByte();
        Assert.Equal(10, newResponse);

        Assert.NotEqual(response, newResponse);
        Assert.NotEqual(response, response2);
    }
}
