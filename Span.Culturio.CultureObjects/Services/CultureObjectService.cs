using Microsoft.EntityFrameworkCore;
using Span.Culturio.CultureObjects.Data;
using Span.Culturio.CultureObjects.Services.Interfaces;
using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.CultureObjects.Services
{
    public class CultureObjectService : ICultureObjectService
    {
        private readonly CulturioDbContext _context;
        private readonly ILogger<CultureObjectService> _logger;

        public CultureObjectService(CulturioDbContext context, ILogger<CultureObjectService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CultureObject> CreateAsync(CreateCultureObjectDto dto)
        {
            _logger.LogInformation("Creating new culture object: {Name}", dto.Name);

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AdminUserId);
            if (!userExists)
            {
                _logger.LogWarning("User with ID {AdminUserId} does not exist", dto.AdminUserId);
                throw new InvalidOperationException($"User with ID {dto.AdminUserId} does not exist");
            }

            var cultureObject = new CultureObject
            {
                Name = dto.Name,
                ContactEmail = dto.ContactEmail,
                Address = dto.Address,
                ZipCode = dto.ZipCode,
                City = dto.City,
                AdminUserId = dto.AdminUserId
            };

            _context.CultureObjects.Add(cultureObject);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Culture object created with ID: {Id}", cultureObject.Id);

            return cultureObject;
        }

        public async Task<List<CultureObject>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all culture objects");

            var cultureObjects = await _context.CultureObjects.ToListAsync();

            _logger.LogInformation("Fetched {Count} culture objects", cultureObjects.Count);

            return cultureObjects;
        }

        public async Task<CultureObject?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching culture object by ID: {Id}", id);

            var cultureObject = await _context.CultureObjects.FindAsync(id);

            if (cultureObject == null)
            {
                _logger.LogWarning("Culture object with ID {Id} not found", id);
            }

            return cultureObject;
        }
    }
}
