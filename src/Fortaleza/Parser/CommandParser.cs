using Fortaleza.Engine;

namespace Fortaleza.Parser;

public enum CommandType
{
    Go, Take, Leave, LeaveAll, Open, Kill, Look, See, Break, Ask, Inventory,
    Give, Quit, Exec, Save, Percent, Weigh, Clear, Unknown
}

public record ParsedCommand(CommandType Type, string Arg1, string Arg2);

public static class CommandParser
{
    private static readonly Dictionary<string, CommandType> Verbs = new()
    {
        ["ATRAVESAR"] = CommandType.Go, ["IR"] = CommandType.Go,
        ["CRUZAR"] = CommandType.Go, ["PASAR"] = CommandType.Go,
        ["GO"] = CommandType.Go, ["WALK"] = CommandType.Go,
        ["TRAVERSE"] = CommandType.Go, ["CROSS"] = CommandType.Go,
        ["ENTER"] = CommandType.Go, ["THROUGH"] = CommandType.Go,
        ["TOMAR"] = CommandType.Take, ["COGER"] = CommandType.Take,
        ["TAKE"] = CommandType.Take, ["GET"] = CommandType.Take, ["PICK"] = CommandType.Take,
        ["SOLTAR"] = CommandType.Leave, ["DEJAR"] = CommandType.Leave,
        ["DROP"] = CommandType.Leave, ["LEAVE"] = CommandType.Leave, ["PUT"] = CommandType.Leave,
        ["ABRIR"] = CommandType.Open, ["OPEN"] = CommandType.Open,
        ["MATAR"] = CommandType.Kill, ["ASESINAR"] = CommandType.Kill,
        ["KILL"] = CommandType.Kill, ["ATTACK"] = CommandType.Kill, ["SLAY"] = CommandType.Kill,
        ["OBSERVAR"] = CommandType.Look, ["MIRAR"] = CommandType.Look,
        ["LOOK"] = CommandType.Look, ["EXAMINE"] = CommandType.Look,
        ["LEER"] = CommandType.See, ["VER"] = CommandType.See,
        ["READ"] = CommandType.See, ["SEE"] = CommandType.See,
        ["ROMPER"] = CommandType.Break, ["FORZAR"] = CommandType.Break,
        ["DESTROZAR"] = CommandType.Break,
        ["BREAK"] = CommandType.Break, ["FORCE"] = CommandType.Break, ["SMASH"] = CommandType.Break,
        ["PREGUNTAR"] = CommandType.Ask, ["INTERROGAR"] = CommandType.Ask,
        ["ASK"] = CommandType.Ask, ["TALK"] = CommandType.Ask, ["QUESTION"] = CommandType.Ask,
        ["INVENTARIO"] = CommandType.Inventory, ["INVENTORY"] = CommandType.Inventory,
        ["ITEMS"] = CommandType.Inventory, ["BAG"] = CommandType.Inventory, ["I"] = CommandType.Inventory,
        ["REGALAR"] = CommandType.Give, ["DAR"] = CommandType.Give,
        ["GIVE"] = CommandType.Give,
        ["ABANDONAR"] = CommandType.Quit, ["TERMINAR"] = CommandType.Quit,
        ["QUIT"] = CommandType.Quit, ["EXIT"] = CommandType.Quit,
        ["EJECUTAR"] = CommandType.Exec, ["EXECUTE"] = CommandType.Exec, ["RUN"] = CommandType.Exec,
        ["SALVAR"] = CommandType.Save, ["SAVE"] = CommandType.Save,
        ["PORCIENTO"] = CommandType.Percent, ["PERCENT"] = CommandType.Percent,
        ["PESAR"] = CommandType.Weigh, ["WEIGH"] = CommandType.Weigh,
        ["CLS"] = CommandType.Clear, ["CLEAR"] = CommandType.Clear,
    };

    private static readonly HashSet<string> StripWords = new()
    {
        "EL", "LA", "LOS", "LAS", "POR", "AL", "UN", "UNA", "THE", "AN", "BY", "INTO", "THROUGH"
    };

    private static readonly HashSet<string> Separators = new()
    {
        "CON", "WITH", "DICIENDO", "RESPONDIENDO", "SAYING", "ANSWERING"
    };

    private static readonly HashSet<string> ObjectPrepositions = new() { "A", "TO" };

    public static ParsedCommand Parse(string input)
    {
        var tokens = StringHelper.Normalize(input)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (tokens.Count == 0 || !Verbs.TryGetValue(tokens[0], out var type))
            return new ParsedCommand(CommandType.Unknown, "", "");

        var first = new List<string>();
        var second = new List<string>();
        var inSecond = false;

        foreach (var token in tokens.Skip(1))
        {
            if (inSecond)
            {
                if (!StripWords.Contains(token)) second.Add(token);
            }
            else if (Separators.Contains(token)
                     || (ObjectPrepositions.Contains(token) && first.Count > 0))
            {
                inSecond = true;
            }
            else if (!StripWords.Contains(token) && !ObjectPrepositions.Contains(token))
            {
                first.Add(token);
            }
        }

        var arg1 = string.Join(' ', first);
        if (type == CommandType.Leave && arg1 is "TODO" or "ALL")
            type = CommandType.LeaveAll;

        return new ParsedCommand(type, arg1, string.Join(' ', second));
    }
}
