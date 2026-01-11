using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Packages.Services.Interfaces
{
    public interface IPackageService
    {
        Task<List<Package>> GetAllAsync();
    }
}
