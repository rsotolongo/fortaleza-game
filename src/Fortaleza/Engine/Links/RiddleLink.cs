using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class RiddleLink : Link
{
    public LocalizedText Riddle { get; }
    public string Answer { get; }

    public RiddleLink(
        LocalizedText name, LocalizedText description,
        LocalizedText riddle, string answer,
        LocalizedText passComment, Room destination)
        : base(name, description, passComment, "", destination)
    {
        Riddle = riddle;
        Answer = answer;
        IsOpen = false;
    }

    public override bool TryOpen(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Output(Localizer.Msg("SolveRiddle") + " " + Riddle.Get());
            Output(Localizer.Msg("SolveRiddleHint"));
            return false;
        }
        if (StringHelper.Normalize(key) == StringHelper.Normalize(Answer))
        {
            IsOpen = true;
            Output(Localizer.Msg("RiddleSolved"));
            return true;
        }
        Output(Localizer.Msg("WrongAnswer"));
        return false;
    }

    public override Room? GetDest() => IsOpen ? Destination : null;
}
