namespace linkedList;

public class Record
{
    public DateOnly Date;
    public List<string> Rows;
    public string Name;

    public Record(string name, DateOnly date, List<string> rows)
    {
        Name = name;
        Date = date;
        Rows = rows;
    }

    public override string ToString()
    {
        var maxLength = Rows.MaxBy(a => a.Length).Length;
        string text = "";
        text += $"Jmeno: {Name}\n";
        text += new string('=', maxLength) + "\n";
        text += $"Datum: {Date}\n";
        text += new string('=', maxLength) + "\n";
        text += string.Join('\n', Rows);
        return text;
    }
}