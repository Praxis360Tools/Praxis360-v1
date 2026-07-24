using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Services;

public sealed class ClientSelectionService : IClientSelectionService
{
    private readonly IClientRepository _clientRepository;

    public ClientSelectionService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository ?? throw new System.ArgumentNullException(nameof(clientRepository));
    }

    public async Task<IReadOnlyCollection<SelectableClient>> GetSelectableClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();

        return clients
            .Select(c => new SelectableClient(
                c.Id,
                c.FirstName,
                c.LastName,
                c.DateOfBirth))
            .ToList()
            .AsReadOnly();
    }
}
