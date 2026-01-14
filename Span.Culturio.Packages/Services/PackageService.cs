using Microsoft.EntityFrameworkCore;
using Span.Culturio.Packages.Data;
using Span.Culturio.Packages.Services.Interfaces;
using Span.Culturio.Packages.Models.Entities;

namespace Span.Culturio.Packages.Services
{
    public class PackageService : IPackageService
    {
        private readonly PackagesDbContext _context;
        private readonly ILogger<PackageService> _logger;

        public PackageService(PackagesDbContext context, ILogger<PackageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Package> Packages, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching packages with culture objects (Page: {Page}, PageSize: {PageSize})", page, pageSize);

            var totalCount = await _context.Packages.CountAsync();

            var packages = await _context.Packages
                .Include(p => p.PackageCultureObjects)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} packages out of {TotalCount}", packages.Count, totalCount);

            return (packages, totalCount);
        }
    }
}
