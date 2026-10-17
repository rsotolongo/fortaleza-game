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

    [Fact]
    public void Parse_LosAndLas_AreNotStrippedLikeTheOriginal()
    {
        var cmd = CommandParser.Parse("tomar las llaves");

        Assert.Equal("LAS LLAVES", cmd.Arg1);
    }

    [Theory]
    [InlineData("coger maza", CommandType.Take)]
    [InlineData("tomar maza", CommandType.Take)]
    [InlineData("take mace", CommandType.Take)]
    [InlineData("take a mace", CommandType.Take)]
    [InlineData("tomar todo", CommandType.TakeAll)]
    [InlineData("take all", CommandType.TakeAll)]
    [InlineData("inventario", CommandType.Inventory)]
    [InlineData("i", CommandType.Inventory)]
    [InlineData("dejar todo", CommandType.LeaveAll)]
    [InlineData("drop all", CommandType.LeaveAll)]
    [InlineData("soltar maza", CommandType.Leave)]
    [InlineData("mirar", CommandType.Look)]
    [InlineData("observar", CommandType.Look)]
    [InlineData("mirar espejo", CommandType.See)]
    [InlineData("ver", CommandType.See)]
    [InlineData("leer libro", CommandType.See)]
    [InlineData("cruzar puerta", CommandType.Go)]
    [InlineData("atravesar puerta", CommandType.Go)]
    [InlineData("pasar puerta", CommandType.Go)]
    [InlineData("abandonar", CommandType.Quit)]
    [InlineData("terminar", CommandType.Quit)]
    [InlineData("porciento", CommandType.Percent)]
    [InlineData("pesar maza", CommandType.Weigh)]
    [InlineData("ejecutar", CommandType.Exec)]
    [InlineData("salvar", CommandType.Save)]
    [InlineData("cls", CommandType.Clear)]
    [InlineData("orinar", CommandType.Piss)]
    [InlineData("miar", CommandType.Piss)]
    [InlineData("destrozar espejo", CommandType.Break)]
    [InlineData("asesinar lobo", CommandType.Kill)]
    [InlineData("interrogar bruja", CommandType.Ask)]
    [InlineData("xyzzy", CommandType.Unknown)]
    [InlineData("", CommandType.Unknown)]
    public void Parse_Verb_ReturnsExpectedType(string input, CommandType expected)
    {
        Assert.Equal(expected, CommandParser.Parse(input).Type);
    }

    [Fact]
    public void Parse_LookWithObject_BecomesSeeOfThatObject()
    {
        var cmd = CommandParser.Parse("mirar espejo grande");

        Assert.Equal(CommandType.See, cmd.Type);
        Assert.Equal("ESPEJO GRANDE", cmd.Arg1);
    }

    [Theory]
    [InlineData("dar escoba a la bruja", "ESCOBA", "BRUJA")]
    [InlineData("regalar escoba a bruja", "ESCOBA", "BRUJA")]
    [InlineData("give broom to the witch", "BROOM", "WITCH")]
    public void Parse_Give_SplitsOnPreposition(string input, string arg1, string arg2)
    {
        var cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Give, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Theory]
    [InlineData("romper espejo con hueso de gato", CommandType.Break, "ESPEJO", "HUESO DE GATO")]
    [InlineData("matar lobo con latigo", CommandType.Kill, "LOBO", "LATIGO")]
    [InlineData("break mirror with cat bone", CommandType.Break, "MIRROR", "CAT BONE")]
    [InlineData("matar lobo", CommandType.Kill, "LOBO", "")]
    public void Parse_KillAndBreak_SplitOnWith(string input, CommandType type, string arg1, string arg2)
    {
        var cmd = CommandParser.Parse(input);

        Assert.Equal(type, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Theory]
    [InlineData("abrir escalera respondiendo treinta y nueve", "ESCALERA", "TREINTA Y NUEVE")]
    [InlineData("abrir puerta diciendo fortaleza", "PUERTA", "FORTALEZA")]
    [InlineData("abrir puerta con fortaleza", "PUERTA", "FORTALEZA")]
    [InlineData("open door saying Fortaleza", "DOOR", "FORTALEZA")]
    [InlineData("open door answering thirty nine", "DOOR", "THIRTY NINE")]
    [InlineData("abrir puerta azul", "PUERTA AZUL", "")]
    public void Parse_Open_SplitsOnAnswerWord(string input, string arg1, string arg2)
    {
        var cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Open, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Fact]
    public void Parse_WithOnlyAppliesToKillBreakAndOpen_NotToTake()
    {
        var cmd = CommandParser.Parse("tomar maza con soga");

        Assert.Equal("MAZA CON SOGA", cmd.Arg1);
        Assert.Equal("", cmd.Arg2);
    }

    [Fact]
    public void Parse_DiacriticsInInput_AreNormalized()
    {
        var cmd = CommandParser.Parse("abrir escalera respondiendo Treinta y nueve peldaños");

        Assert.Equal("TREINTA Y NUEVE PELDANOS", cmd.Arg2);
    }
}
