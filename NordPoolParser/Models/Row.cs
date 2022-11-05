namespace NordPoolParser.Models;

public class Row
{
    public IReadOnlyList<Column> Columns { get; init; } = Array.Empty<Column>();
    public DateTime EndTime { get; set; }
}