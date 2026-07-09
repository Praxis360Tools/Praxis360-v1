namespace Praxis360_v1.Models;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public bool Favorite { get; set; }

    public string Description { get; set; } = string.Empty;
}