namespace linkedList;

public class Command
{
    public Func<string[],bool> Func;
    public string Description;
    public bool HasArgs;
    public string[]? Args => HasArgs ? _argsFunction is not null ? _argsFunction() : null : null;
    private Func<string[]>? _argsFunction;

    public Command(Func<string[], bool> func, string description, bool hasArgses = false, Func<string[]>? possibilitiesArgs = null)
    {
        Func = func;
        Description = description;
        HasArgs = hasArgses;
        _argsFunction = possibilitiesArgs;
    }
}