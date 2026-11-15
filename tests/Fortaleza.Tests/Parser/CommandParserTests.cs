using Fortaleza.Parser;

namespace Fortaleza.Tests.Parser;

public sealed class CommandParserTests
{
    [Fact]
    public void ParseSpanishArticlesAreStripped()
    {
        ParsedCommand cmd = CommandParser.Parse("ir por la puerta principal");

        Assert.Equal(CommandType.Go, cmd.Type);
        Assert.Equal("PUERTA PRINCIPAL", cmd.Arg1);
    }

    [Fact]
    public void ParseEnglishArticlesAreStripped()
    {
        ParsedCommand cmd = CommandParser.Parse("go through the main door");

        Assert.Equal(CommandType.Go, cmd.Type);
        Assert.Equal("MAIN DOOR", cmd.Arg1);
    }

    [Fact]
    public void ParseLosAndLasAreNotStrippedLikeTheOriginal()
    {
        ParsedCommand cmd = CommandParser.Parse("tomar las llaves");

        Assert.Equal("LAS LLAVES", cmd.Arg1);
    }

    [Theory]
    [InlineData("coger maza", nameof(CommandType.Take))]
    [InlineData("tomar maza", nameof(CommandType.Take))]
    [InlineData("take mace", nameof(CommandType.Take))]
    [InlineData("take a mace", nameof(CommandType.Take))]
    [InlineData("tomar todo", nameof(CommandType.TakeAll))]
    [InlineData("take all", nameof(CommandType.TakeAll))]
    [InlineData("inventario", nameof(CommandType.Inventory))]
    [InlineData("i", nameof(CommandType.Inventory))]
    [InlineData("dejar todo", nameof(CommandType.LeaveAll))]
    [InlineData("drop all", nameof(CommandType.LeaveAll))]
    [InlineData("soltar maza", nameof(CommandType.Leave))]
    [InlineData("mirar", nameof(CommandType.Look))]
    [InlineData("observar", nameof(CommandType.Look))]
    [InlineData("mirar espejo", nameof(CommandType.See))]
    [InlineData("ver", nameof(CommandType.See))]
    [InlineData("leer libro", nameof(CommandType.See))]
    [InlineData("cruzar puerta", nameof(CommandType.Go))]
    [InlineData("atravesar puerta", nameof(CommandType.Go))]
    [InlineData("pasar puerta", nameof(CommandType.Go))]
    [InlineData("abandonar", nameof(CommandType.Quit))]
    [InlineData("terminar", nameof(CommandType.Quit))]
    [InlineData("porciento", nameof(CommandType.Percent))]
    [InlineData("pesar maza", nameof(CommandType.Weigh))]
    [InlineData("ejecutar", nameof(CommandType.Exec))]
    [InlineData("salvar", nameof(CommandType.Save))]
    [InlineData("cls", nameof(CommandType.Clear))]
    [InlineData("orinar", nameof(CommandType.Piss))]
    [InlineData("miar", nameof(CommandType.Piss))]
    [InlineData("destrozar espejo", nameof(CommandType.Break))]
    [InlineData("asesinar lobo", nameof(CommandType.Kill))]
    [InlineData("interrogar bruja", nameof(CommandType.Ask))]
    [InlineData("xyzzy", nameof(CommandType.Unknown))]
    [InlineData("", nameof(CommandType.Unknown))]
    public void ParseVerbReturnsExpectedType(string input, string expected)
    {
        Assert.Equal(Enum.Parse<CommandType>(expected), CommandParser.Parse(input).Type);
    }

    [Fact]
    public void ParseLookWithObjectBecomesSeeOfThatObject()
    {
        ParsedCommand cmd = CommandParser.Parse("mirar espejo grande");

        Assert.Equal(CommandType.See, cmd.Type);
        Assert.Equal("ESPEJO GRANDE", cmd.Arg1);
    }

    [Theory]
    [InlineData("dar escoba a la bruja", "ESCOBA", "BRUJA")]
    [InlineData("regalar escoba a bruja", "ESCOBA", "BRUJA")]
    [InlineData("give broom to the witch", "BROOM", "WITCH")]
    public void ParseGiveSplitsOnPreposition(string input, string arg1, string arg2)
    {
        ParsedCommand cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Give, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Theory]
    [InlineData("romper espejo con hueso de gato", nameof(CommandType.Break), "ESPEJO", "HUESO DE GATO")]
    [InlineData("matar lobo con latigo", nameof(CommandType.Kill), "LOBO", "LATIGO")]
    [InlineData("break mirror with cat bone", nameof(CommandType.Break), "MIRROR", "CAT BONE")]
    [InlineData("matar lobo", nameof(CommandType.Kill), "LOBO", "")]
    public void ParseKillAndBreakSplitOnWith(string input, string type, string arg1, string arg2)
    {
        ParsedCommand cmd = CommandParser.Parse(input);

        Assert.Equal(Enum.Parse<CommandType>(type), cmd.Type);
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
    public void ParseOpenSplitsOnAnswerWord(string input, string arg1, string arg2)
    {
        ParsedCommand cmd = CommandParser.Parse(input);

        Assert.Equal(CommandType.Open, cmd.Type);
        Assert.Equal(arg1, cmd.Arg1);
        Assert.Equal(arg2, cmd.Arg2);
    }

    [Fact]
    public void ParseWithOnlyAppliesToKillBreakAndOpenNotToTake()
    {
        ParsedCommand cmd = CommandParser.Parse("tomar maza con soga");

        Assert.Equal("MAZA CON SOGA", cmd.Arg1);
        Assert.Equal(string.Empty, cmd.Arg2);
    }

    [Fact]
    public void ParseDiacriticsInInputAreNormalized()
    {
        ParsedCommand cmd = CommandParser.Parse("abrir escalera respondiendo Treinta y nueve peldaños");

        Assert.Equal("TREINTA Y NUEVE PELDANOS", cmd.Arg2);
    }
}
