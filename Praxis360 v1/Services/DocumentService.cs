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
        },

        new()
        {
            Title = "Compte Professionnel",
            Category = "Banque",
            FileType = "PDF",
            FileSize = 245860,
            Favorite = true,
            Description = "Convention bancaire",
            DateAdded = new DateTime(2026, 2, 8)
        },

        new()
        {
            Title = "Prêt Professionnel",
            Category = "Banque",
            FileType = "PDF",
            FileSize = 412365,
            Favorite = false,
            Description = "Contrat de crédit",
            DateAdded = new DateTime(2025, 12, 15)
        },

        new()
        {
            Title = "Carte Grise BMW",
            Category = "Véhicules",
            FileType = "PDF",
            FileSize = 89452,
            Favorite = false,
            Description = "Certificat d'immatriculation",
            DateAdded = new DateTime(2026, 4, 2)
        },

        new()
        {
            Title = "Contrôle Technique",
            Category = "Véhicules",
            FileType = "PDF",
            FileSize = 114520,
            Favorite = false,
            Description = "Contrôle périodique",
            DateAdded = new DateTime(2026, 3, 30)
        },

        new()
        {
            Title = "Acte de Propriété",
            Category = "Habitation",
            FileType = "PDF",
            FileSize = 658420,
            Favorite = true,
            Description = "Maison principale",
            DateAdded = new DateTime(2025, 6, 10)
        },

        new()
        {
            Title = "Facture Électricité",
            Category = "Énergie",
            FileType = "PDF",
            FileSize = 75640,
            Favorite = false,
            Description = "Facture mensuelle",
            DateAdded = new DateTime(2026, 4, 10)
        },

        new()
        {
            Title = "Facture Gaz",
            Category = "Énergie",
            FileType = "PDF",
            FileSize = 70215,
            Favorite = false,
            Description = "Consommation gaz",
            DateAdded = new DateTime(2026, 3, 10)
        },

        new()
        {
            Title = "Carte d'Identité",
            Category = "Administratif",
            FileType = "PDF",
            FileSize = 95412,
            Favorite = true,
            Description = "Document d'identité",
            DateAdded = new DateTime(2024, 8, 5)
        },

        new()
        {
            Title = "Passeport",
            Category = "Administratif",
            FileType = "PDF",
            FileSize = 125480,
            Favorite = false,
            Description = "Passeport biométrique",
            DateAdded = new DateTime(2025, 7, 18)
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

    public IEnumerable<Document> GetFilteredDocuments(
        string? search = null,
        string? category = null)
    {
        IEnumerable<Document> query = _documents;

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(d =>
                (d.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (d.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (d.Category?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (d.FileType?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d =>
                d.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }
}