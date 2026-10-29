using CustomerOrderReport.PL.Arguments;
using CustomerOrderReport.PL.Exceptions;

namespace CustomerOrderReport.Tests.PL;

public class ArgsParserTests
{
    private readonly ArgsParser _parser = new();

    [Theory]
    [InlineData("console")]
    [InlineData("file")]
    public void Parse_ModeAndIds_ReturnsModeAndIds(string mode)
    {
        var request = _parser.Parse([mode, "1010", "1020"]);

        Assert.Equal(mode, request.Mode);
        Assert.Equal([1010L, 1020L], request.ClientIds);
    }

    [Fact]
    public void Parse_WithoutMode_DefaultsToConsole()
    {
        var request = _parser.Parse(["1010"]);

        Assert.Equal(ReportModes.Console, request.Mode);
        Assert.Equal([1010L], request.ClientIds);
    }

    [Fact]
    public void Parse_DuplicateIds_AreRemoved()
    {
        var request = _parser.Parse(["console", "1010", "1010", "1020", "1010"]);

        Assert.Equal([1010L, 1020L], request.ClientIds);
    }

    [Fact]
    public void Parse_NoArguments_Throws()
    {
        Assert.Throws<InvalidArgumentException>(() => _parser.Parse([]));
    }

    [Fact]
    public void Parse_UnknownMode_Throws()
    {
        var exception = Assert.Throws<InvalidArgumentException>(() => _parser.Parse(["pdf", "1010"]));

        Assert.Contains("pdf", exception.Message);
    }

    [Fact]
    public void Parse_ModeWithoutIds_Throws()
    {
        Assert.Throws<InvalidArgumentException>(() => _parser.Parse(["file"]));
    }

    [Fact]
    public void Parse_IdIsNotANumber_Throws()
    {
        var exception = Assert.Throws<InvalidArgumentException>(() => _parser.Parse(["console", "1010", "abc"]));

        Assert.Contains("abc", exception.Message);
    }
}
