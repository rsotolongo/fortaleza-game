using Fortaleza.Engine;

namespace Fortaleza.Parser;

public enum CommandType
{
    Go, Take, TakeAll, Leave, LeaveAll, Open, Kill, Look, See, Break, Ask, Inventory,
    Give, Quit, Exec, Save, Percent, Weigh, Clear, Piss, Unknown
}

public record ParsedCommand(CommandType Type, string Arg1, string Arg2);

public static class CommandParser
{
    private static readonly Dictionary<string, CommandType> Verbs = new()
    {
        ["ATRAVESAR"] = CommandType.Go, ["IR"] = CommandType.Go,
        ["CRUZAR"] = CommandType.Go, ["PASAR"] = CommandType.Go,
        ["GO"] = CommandType.Go, ["WALK"] = CommandType.Go,
        ["TRAVERSE"] = CommandType.Go, ["CROSS"] = CommandType.Go, ["ENTER"] = CommandType.Go,
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
        ["REGALAR"] = CommandType.Give, ["DAR"] = CommandType.Give, ["GIVE"] = CommandType.Give,
        ["ABANDONAR"] = CommandType.Quit, ["TERMINAR"] = CommandType.Quit,
        ["QUIT"] = CommandType.Quit, ["EXIT"] = CommandType.Quit,
        ["EJECUTAR"] = CommandType.Exec, ["EXECUTE"] = CommandType.Exec, ["RUN"] = CommandType.Exec,
        ["SALVAR"] = CommandType.Save, ["SAVE"] = CommandType.Save,
        ["PORCIENTO"] = CommandType.Percent, ["PERCENT"] = CommandType.Percent,
        ["PESAR"] = CommandType.Weigh, ["WEIGH"] = CommandType.Weigh,
        ["CLS"] = CommandType.Clear, ["CLEAR"] = CommandType.Clear,
        ["ORINAR"] = CommandType.Piss, ["MIAR"] = CommandType.Piss,
    };

    private static readonly HashSet<string> Articles = new()
    {
        "LA", "EL", "POR", "AL", "THE", "AN", "BY", "INTO", "THROUGH"
    };

    private static readonly HashSet<string> WithWords = new() { "CON", "WITH" };

    private static readonly HashSet<string> OpenWords = new()
    {
        "CON", "WITH", "RESPONDIENDO", "ANSWERING", "DICIENDO", "SAYING"
    };

    private static readonly HashSet<string> ToWords = new() { "A", "TO" };

    private static readonly HashSet<string> AllWords = new() { "TODO", "ALL" };

    public static ParsedCommand Parse(string input)
    {
        var tokens = StringHelper.Normalize(input)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => !Articles.Contains(t))
            .ToList();

        if (tokens.Count == 0 || !Verbs.TryGetValue(tokens[0], out var type))
            return new ParsedCommand(CommandType.Unknown, "", "");

        var rest = tokens.Skip(1).ToList();
        var separators = type switch
        {
            CommandType.Open => OpenWords,
            CommandType.Kill or CommandType.Break => WithWords,
            CommandType.Give => ToWords,
            _ => null
        };

        if (type != CommandType.Give)
            rest.RemoveAll(t => t == "A");

        var cut = separators is null ? -1 : rest.FindIndex(separators.Contains);
        var arg1 = string.Join(' ', cut < 0 ? rest : rest.Take(cut));
        var arg2 = cut < 0 ? "" : string.Join(' ', rest.Skip(cut + 1));

        type = type switch
        {
            CommandType.Look when arg1 != "" => CommandType.See,
            CommandType.Take when AllWords.Contains(arg1) => CommandType.TakeAll,
            CommandType.Leave when AllWords.Contains(arg1) => CommandType.LeaveAll,
            _ => type
        };

        return new ParsedCommand(type, arg1, arg2);
    }
}
