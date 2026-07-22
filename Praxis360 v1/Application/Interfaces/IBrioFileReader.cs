using System.IO;
using System.Threading.Tasks;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Interfaces;

public interface IBrioFileReader
{
    Task<BrioFileReadResult> ReadAsync(Stream stream);
}
