using Praxis360_v1.Models;

namespace Praxis360_v1.Services;

public class DocumentService
{
    private readonly List<Document> _documents =
    [
        new()
        {
            Title = "Assurance Vie",
            Category = "Assurances",
            FileType = "PDF",
            FileSize = 284563,
            Favorite = true,
            Description = "Contrat assurance vie",
            DateAdded = new DateTime(2026, 3, 12)
        },

        new()
        {
            Title = "Assurance Auto",
            Category = "Assurances",
            FileType = "PDF",
            FileSize = 195420,
            Favorite = false,
            Description = "Contrat véhicule",
            DateAdded = new DateTime(2026, 1, 18)
        },

        new()
        {
            Title = "Assurance Habitation",
            Category = "Assurances",
            FileType = "PDF",
            FileSize = 321450,
            Favorite = false,
            Description = "Contrat habitation",
            DateAdded = new DateTime(2025, 11, 2)
        },

        new()
        {
            Title = "Protection Juridique",
            Category = "Assurances",
            FileType = "PDF",
            FileSize = 154820,
            Favorite = true,
            Description = "Police protection juridique",
            DateAdded = new DateTime(2025, 9, 20)
        }
    ];

    private readonly List<DocumentCategory> _categories =
    [
        new()
        {
            Title = "Assurances",
            Url = "/espace/assurances",
            Icon = "shield"
        },

        new()
        {
            Title = "Banque",
            Url = "#",
            Icon = "bank"
        },

        new()
        {
            Title = "Véhicules",
            Url = "#",
            Icon = "car"
        },

        new()
        {
            Title = "Habitation",
            Url = "#",
            Icon = "house"
        },

        new()
        {
            Title = "Énergie",
            Url = "#",
            Icon = "lightning"
        },

        new()
        {
            Title = "Administratif",
            Url = "#",
            Icon = "folder"
        }
    ];

    public IEnumerable<Document> GetDocuments()
    {
        return _documents;
    }

    public IEnumerable<DocumentCategory> GetCategories()
    {
        return _categories;
    }

    public IEnumerable<Document> GetDocumentsByCategory(string category)
    {
        return _documents.Where(d =>
            d.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Document> SearchDocuments(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return _documents;

        return _documents.Where(d =>
            d.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
            || d.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
            || d.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
    }
}