using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.CultureObjects.Services.Interfaces
{
    public interface ICultureObjectService
    {
        Task<CultureObject> CreateAsync(CreateCultureObjectDto dto);
        Task<List<CultureObject>> GetAllAsync();
        Task<CultureObject?> GetByIdAsync(int id);
    }
}
