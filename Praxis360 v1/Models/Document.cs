namespace Praxis360_v1.Models;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = "";

    public string Category { get; set; } = "";

    public string FileType { get; set; } = "";

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public string FilePath { get; set; } = "";

    public string FileName { get; set; } = "";

    public long FileSize { get; set; }
}