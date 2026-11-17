using Fortaleza.Engine;

namespace Fortaleza.Parser;

internal static class CommandParser
{
    private static readonly Dictionary<string, CommandType> Verbs = new(StringComparer.Ordinal)
    {
        ["ATRAVESAR"] = CommandType.Go,
        ["IR"] = CommandType.Go,
        ["CRUZAR"] = CommandType.Go,
        ["PASAR"] = CommandType.Go,
        ["GO"] = CommandType.Go,
        ["WALK"] = CommandType.Go,
        ["TRAVERSE"] = CommandType.Go,
        ["CROSS"] = CommandType.Go,
        ["ENTER"] = CommandType.Go,
        ["TOMAR"] = CommandType.Take,
        ["COGER"] = CommandType.Take,
        ["TAKE"] = CommandType.Take,
        ["GET"] = CommandType.Take,
        ["PICK"] = CommandType.Take,
        ["SOLTAR"] = CommandType.Leave,
        ["DEJAR"] = CommandType.Leave,
        ["DROP"] = CommandType.Leave,
        ["LEAVE"] = CommandType.Leave,
        ["PUT"] = CommandType.Leave,
        ["ABRIR"] = CommandType.Open,
        ["OPEN"] = CommandType.Open,
        ["MATAR"] = CommandType.Kill,
        ["ASESINAR"] = CommandType.Kill,
        ["KILL"] = CommandType.Kill,
        ["ATTACK"] = CommandType.Kill,
        ["SLAY"] = CommandType.Kill,
        ["OBSERVAR"] = CommandType.Look,
        ["MIRAR"] = CommandType.Look,
        ["LOOK"] = CommandType.Look,
        ["EXAMINE"] = CommandType.Look,
        ["LEER"] = CommandType.See,
        ["VER"] = CommandType.See,
        ["READ"] = CommandType.See,
        ["SEE"] = CommandType.See,
        ["ROMPER"] = CommandType.Break,
        ["FORZAR"] = CommandType.Break,
        ["DESTROZAR"] = CommandType.Break,
        ["BREAK"] = CommandType.Break,
        ["FORCE"] = CommandType.Break,
        ["SMASH"] = CommandType.Break,
        ["PREGUNTAR"] = CommandType.Ask,
        ["INTERROGAR"] = CommandType.Ask,
        ["ASK"] = CommandType.Ask,
        ["TALK"] = CommandType.Ask,
        ["QUESTION"] = CommandType.Ask,
        ["INVENTARIO"] = CommandType.Inventory,
        ["INVENTORY"] = CommandType.Inventory,
        ["ITEMS"] = CommandType.Inventory,
        ["BAG"] = CommandType.Inventory,
        ["I"] = CommandType.Inventory,
        ["REGALAR"] = CommandType.Give,
        ["DAR"] = CommandType.Give,
        ["GIVE"] = CommandType.Give,
        ["ABANDONAR"] = CommandType.Quit,
        ["TERMINAR"] = CommandType.Quit,
        ["QUIT"] = CommandType.Quit,
        ["EXIT"] = CommandType.Quit,
        ["EJECUTAR"] = CommandType.Exec,
        ["EXECUTE"] = CommandType.Exec,
        ["RUN"] = CommandType.Exec,
        ["SALVAR"] = CommandType.Save,
        ["SAVE"] = CommandType.Save,
        ["PORCIENTO"] = CommandType.Percent,
        ["PERCENT"] = CommandType.Percent,
        ["PESAR"] = CommandType.Weigh,
        ["WEIGH"] = CommandType.Weigh,
        ["CLS"] = CommandType.Clear,
        ["CLEAR"] = CommandType.Clear,
        ["ORINAR"] = CommandType.Piss,
        ["MIAR"] = CommandType.Piss,
    };

    private static readonly HashSet<string> Articles = new(
        ["LA", "EL", "POR", "AL", "THE", "AN", "BY", "INTO", "THROUGH"],
        StringComparer.Ordinal);

    private static readonly HashSet<string> WithWords = new(
        ["CON", "WITH"],
        StringComparer.Ordinal);

    private static readonly HashSet<string> OpenWords = new(
        ["CON", "WITH", "RESPONDIENDO", "ANSWERING", "DICIENDO", "SAYING"],
        StringComparer.Ordinal);

    private static readonly HashSet<string> ToWords = new(
        ["A", "TO"],
        StringComparer.Ordinal);

    private static readonly Dictionary<CommandType, HashSet<string>> Separators = new()
    {
        [CommandType.Open] = OpenWords,
        [CommandType.Kill] = WithWords,
        [CommandType.Break] = WithWords,
        [CommandType.Give] = ToWords,
    };

    private static readonly HashSet<string> AllWords = new(
        ["TODO", "ALL"],
        StringComparer.Ordinal);

    public static ParsedCommand Parse(string input)
    {
        var tokens = StringHelper.Normalize(input)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(static t => !Articles.Contains(t))
            .ToList();

        if (tokens.Count is 0 || !Verbs.TryGetValue(tokens[0], out CommandType type))
        {
            return new ParsedCommand(CommandType.Unknown, string.Empty, string.Empty);
        }

        var rest = tokens.Skip(1).ToList();
        HashSet<string>? separators = Separators.GetValueOrDefault(type);

        if (type is not CommandType.Give)
        {
            rest.RemoveAll(static t => t is "A");
        }

        int cut = separators is null ? -1 : rest.FindIndex(separators.Contains);
        string arg1 = string.Join(' ', cut < 0 ? rest : rest.Take(cut));
        string arg2 = cut < 0 ? string.Empty : string.Join(' ', rest.Skip(cut + 1));

        type = Refine(type, arg1);

        return new ParsedCommand(type, arg1, arg2);
    }

    private static CommandType Refine(CommandType type, string arg1)
    {
        return (type, string.IsNullOrEmpty(arg1), AllWords.Contains(arg1)) switch
        {
            (CommandType.Look, false, _) => CommandType.See,
            (CommandType.Take, _, true) => CommandType.TakeAll,
            (CommandType.Leave, _, true) => CommandType.LeaveAll,
            _ => type,
        };
    }
}
