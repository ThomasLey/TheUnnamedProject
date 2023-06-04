namespace TheUnnamedProject.Core.Model;

public class Filemap
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid? Parent { get; set; }
    public required string? StorePattern { get; set; }
    public string? DocTypes { get; set; }
}