namespace TheUnnamedProject.Core.Model;

public class DocumentType
{
    public required Guid? Id { get; set; }
    public required string? Name { get; set; }
    public required string? TitlePattern { get; set; }
    public IEnumerable<FieldType> Fields { get; set; }
}