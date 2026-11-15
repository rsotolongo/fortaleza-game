using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

internal sealed class RiddleLink : Link
{
    public RiddleLink(
        LocalizedText name,
        LocalizedText description,
        LocalizedText riddle,
        string answer,
        LocalizedText passComment,
        Room destination)
        : base(name, description, passComment, string.Empty, destination)
    {
        Riddle = riddle;
        Answer = answer;
        IsOpen = false;
    }

    public LocalizedText Riddle { get; }
    public string Answer { get; }
    public string AltAnswer { get; init; } = string.Empty;

    public override bool TryOpen(string key)
    {
        string typed = StringHelper.Normalize(key);
        if (typed == StringHelper.Normalize(Answer)
            || (!string.IsNullOrEmpty(AltAnswer) && typed == StringHelper.Normalize(AltAnswer)))
        {
            IsOpen = true;
            Output(Localizer.Msg("RiddleSolved"));
            return true;
        }
        Output(Localizer.Msg(string.IsNullOrEmpty(key) ? "SolveRiddle" : "WrongAnswer"));
        Output(Riddle.Get());
        return false;
    }

    public override Room? GetDest()
    {
        return IsOpen ? Destination : null;
    }
}
