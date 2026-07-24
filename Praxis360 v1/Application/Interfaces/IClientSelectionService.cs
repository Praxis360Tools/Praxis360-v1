using System.Collections.Generic;
using System.Threading.Tasks;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Interfaces;

public interface IClientSelectionService
{
    Task<IReadOnlyCollection<SelectableClient>> GetSelectableClientsAsync();
}
