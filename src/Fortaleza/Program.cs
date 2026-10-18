using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Seleccione idioma / Select language:");
Console.WriteLine("  1. Español");
Console.WriteLine("  2. English");
var languageKey = ReadChoice();
Localizer.Current = languageKey == '2' ? Language.English : Language.Spanish;
ClearScreen();

Console.WriteLine(Localizer.Msg("SelectGame"));
Console.WriteLine("  1. La Fortaleza I");
Console.WriteLine("  2. La Fortaleza II");
var gameKey = ReadChoice();
ClearScreen();

GameEngine engine = gameKey == '2' ? Fort2Builder.Build() : Fort1Builder.Build();
engine.Start();
engine.Player.Output("");

var hour = DateTime.Now.Hour;
engine.Player.Output(Localizer.Msg(hour < 12 ? "GoodMorning" : hour > 18 ? "GoodEvening" : "GoodAfternoon"));

while (!engine.CheckGoal() && !engine.IsOver)
{
    engine.AskPlayer();
    engine.ProcessWish();
}

if (engine.CheckGoal())
{
    if (engine is Fort2)
    {
        ShowPage(Fort2.Escape);
        ClearScreen();
        ShowPage(Fort2.EpilogueFirstPage);
        ReadChoice();
        ClearScreen();
        ShowPage(Fort2.EpilogueLastPage);
    }
    else
    {
        ClearScreen();
        engine.Player.Output(Localizer.Msg("Victory1_Fort1"));
        engine.Player.Output(Localizer.Msg("Victory2_Fort1"));
        engine.Player.Output(Localizer.Msg("Victory3_Fort1"));
        engine.Player.Output(Localizer.Msg("Victory4_Fort1"));
    }
}
else
{
    Console.Beep();
    engine.Player.Output(Localizer.Msg("Defeat"));
}

engine.Player.Output("");
engine.Player.Output(Localizer.Msg("SaveTrack"));
if (char.ToUpperInvariant(ReadChoice()) is 'S' or 'Y')
{
    engine.Player.Output(Localizer.Msg("SaveTrackTo") + GameEngine.DefaultTrackFile);
    engine.SaveTrack(GameEngine.DefaultTrackFile);
}

static char ReadChoice() => Console.IsInputRedirected ? (char)Console.Read() : Console.ReadKey(true).KeyChar;

void ShowPage(IReadOnlyList<LocalizedText> lines)
{
    foreach (var line in lines)
        engine.Player.Output(line.Get());
}

static void ClearScreen()
{
    if (!Console.IsOutputRedirected)
        Console.Clear();
}
