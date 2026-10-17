using Fortaleza.Parser;
using Xunit;

namespace Fortaleza.Tests.Parser;

public class CommandParserTests
{
    [Fact]
    public void Parse_SpanishArticles_AreStripped()
    {
        var cmd = CommandParser.Parse("ir por la puerta principal");

        Assert.Equal(CommandType.Go, cmd.Type);
        Assert.Equal("PUERTA PRINCIPAL", cmd.Arg1);
    }

    [Fact]
    public void Parse_EnglishArticles_AreStripped()
    {
        var cmd = CommandParser.Parse("go through the main door");

        Assert.Equal(CommandType.Go, cmd.Type);
        Assert.Equal("MAIN DOOR", cmd.Arg1);
    }

    [Theory]
    [InlineData("coger maza", CommandType.Take)]
    [InlineData("take mace", CommandType.Take)]
    [InlineData("take a mace", CommandType.Take)]
    [InlineData("inventario", CommandType.Inventory)]
    [InlineData("i", CommandType.Inventory)]
    [InlineData("dejar todo", CommandType.LeaveAll)]
    [InlineData("drop all", CommandType.LeaveAll)]
    [InlineData("xyzzy", CommandType.Unknown)]
    [InlineData("", CommandType.Unknown)]
    public void Parse_Verb_ReturnsExpectedType(string input, CommandType expected)
    {
        Assert.Equal(expected, CommandParser.Parse(input).Type);
    }

    [Theory]
    [InlineData("dar escoba a la bruja", "ESCOBA", "BRUJA")]
    [InlineData("give broom to the witch", "BROOM", "WITCH")]
    public void Parse_Give_SplitsOnPreposition(string input, string arg1, string arg2)
    {
        var cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Give, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Fact]
    public void Parse_BreakWithCon_SplitsObjectAndTool()
    {
        var cmd = CommandParser.Parse("romper espejo con hueso de gato");

        Assert.Equal(CommandType.Break, cmd.Type);
        Assert.Equal("ESPEJO", cmd.Arg1);
        Assert.Equal("HUESO DE GATO", cmd.Arg2);
    }

    [Theory]
    [InlineData("abrir escalera respondiendo treinta y nueve", "ESCALERA", "TREINTA Y NUEVE")]
    [InlineData("abrir puerta diciendo fortaleza", "PUERTA", "FORTALEZA")]
    [InlineData("open door saying Fortaleza", "DOOR", "FORTALEZA")]
    public void Parse_Open_SplitsOnAnswerWord(string input, string arg1, string arg2)
    {
        var cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Open, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Fact]
    public void Parse_DiacriticsInInput_AreNormalized()
    {
        var cmd = CommandParser.Parse("abrir escalera respondiendo Treinta y nueve peldaños");

        Assert.Equal("TREINTA Y NUEVE PELDANOS", cmd.Arg2);
    }
}
