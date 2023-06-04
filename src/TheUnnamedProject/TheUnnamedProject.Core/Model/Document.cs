namespace TheUnnamedProject.Core.Model;

public class Document
{
    public required Guid Id { get; set; }
    public required string? DocumentType { get; set; }
    public required Guid FilemapId { get; set; }
    public required string? RelativeFileName { get; set; }
    public required string? Hash { get; set; }

    public Dictionary<string, string>? UserProperties { get; set; }
    public Dictionary<string, string>? DocumentProperties { get; set; }
}