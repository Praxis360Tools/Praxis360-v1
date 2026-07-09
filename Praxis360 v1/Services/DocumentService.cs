using Praxis360_v1.Models;

namespace Praxis360_v1.Services;

public class DocumentService
{
private readonly List<Document>
	_documents =
	[
	new()
	{
	Title = "Assurance Vie",
	Category = "Assurances",
	FileType = "PDF"
	},

	new()
	{
	Title = "Assurance Auto",
	Category = "Assurances",
	FileType = "PDF"
	},

	new()
	{
	Title = "Assurance Habitation",
	Category = "Assurances",
	FileType = "PDF"
	},

	new()
	{
	Title = "Protection Juridique",
	Category = "Assurances",
	FileType = "PDF"
	}
	];

	private readonly List<DocumentCategory>
		_categories =
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

		public IEnumerable<Document>
			GetDocuments()
			{
			return _documents;
			}

			public IEnumerable<DocumentCategory> GetCategories()
    {
        return _categories;
    }
}