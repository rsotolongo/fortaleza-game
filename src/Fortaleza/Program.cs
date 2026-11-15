using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Seleccione idioma / Select language:");
Console.WriteLine("  1. Español");
Console.WriteLine("  2. English");
char languageKey = ReadChoice();
Localizer.Current = languageKey == '2' ? Language.English : Language.Spanish;
ClearScreen();

Console.WriteLine(Localizer.Msg("SelectGame"));
Console.WriteLine("  1. La Fortaleza I");
Console.WriteLine("  2. La Fortaleza II");
char gameKey = ReadChoice();
ClearScreen();

GameEngine engine = gameKey == '2' ? Fort2Builder.Build() : Fort1Builder.Build();
engine.Start();
Thing.Output(string.Empty);

Thing.Output(Localizer.Msg(GreetingKey(TimeProvider.System.GetLocalNow().Hour)));

while (!engine.CheckGoal() && !engine.IsOver)
{
    engine.AskPlayer();
    engine.ProcessWish();
}

ShowOutcome(engine);

Thing.Output(string.Empty);
Thing.Output(Localizer.Msg("SaveTrack"));
if (char.ToUpperInvariant(ReadChoice()) is 'S' or 'Y')
{
    Thing.Output(Localizer.Msg("SaveTrackTo") + GameEngine.DefaultTrackFile);
    engine.SaveTrack(GameEngine.DefaultTrackFile);
}

static char ReadChoice()
{
    if (!Console.IsInputRedirected)
    {
        return Console.ReadKey(intercept: true).KeyChar;
    }

    int next = Console.Read();
    return next < 0 ? '\0' : Convert.ToChar(next);
}

static string GreetingKey(int hour)
{
    if (hour < 12)
    {
        return "GoodMorning";
    }

    return hour > 18 ? "GoodEvening" : "GoodAfternoon";
}

static void ShowOutcome(GameEngine engine)
{
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
            Thing.Output(Localizer.Msg("Victory1_Fort1"));
            Thing.Output(Localizer.Msg("Victory2_Fort1"));
            Thing.Output(Localizer.Msg("Victory3_Fort1"));
            Thing.Output(Localizer.Msg("Victory4_Fort1"));
        }
    }
    else
    {
        Console.Beep();
        Thing.Output(Localizer.Msg("Defeat"));
    }
}

static void ShowPage(IReadOnlyList<LocalizedText> lines)
{
    foreach (LocalizedText line in lines)
    {
        Thing.Output(line.Get());
    }
}

static void ClearScreen()
{
    if (Console.IsOutputRedirected)
    {
        return;
    }

    Console.Clear();
}
