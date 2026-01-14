using Span.Culturio.Packages.Models.Entities;

namespace Span.Culturio.Packages.Services.Interfaces
{
    public interface IPackageService
    {
        Task<(List<Package> Packages, int TotalCount)> GetAllAsync(int page, int pageSize);
    }
}
