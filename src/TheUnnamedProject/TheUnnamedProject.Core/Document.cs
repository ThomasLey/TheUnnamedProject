namespace TheUnnamedProject.Core;

public class Document
{
    public string? DocumentType { get; set; }
    public string? Filemap { get; set; }
    public string? Title { get; set; }
    public string? RelativeFileName { get; set; }

    public Dictionary<string, string>? Properties { get; set; }
    public string? OriginalTitle { get; set; }
}