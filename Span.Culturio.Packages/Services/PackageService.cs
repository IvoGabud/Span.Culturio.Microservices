using Microsoft.EntityFrameworkCore;
using Span.Culturio.Packages.Data;
using Span.Culturio.Packages.Services.Interfaces;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Packages.Services
{
    public class PackageService : IPackageService
    {
        private readonly CulturioDbContext _context;
        private readonly ILogger<PackageService> _logger;

        public PackageService(CulturioDbContext context, ILogger<PackageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Package>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all packages with culture objects");

            var packages = await _context.Packages
                .Include(p => p.PackageCultureObjects)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} packages", packages.Count);

            return packages;
        }
    }
}
