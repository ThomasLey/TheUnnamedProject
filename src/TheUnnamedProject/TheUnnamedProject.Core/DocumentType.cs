namespace TheUnnamedProject.Core;

public class DocumentType
{
    public string? Name { get; set; }
    public string? TitlePattern { get; set; }
    public IEnumerable<FieldType> Fields { get; set; }
}