using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Span.Culturio.Packages.Models.DTOs;
using Span.Culturio.Packages.Services.Interfaces;

namespace Span.Culturio.Packages.Controllers
{
    [Route("packages")]
    [ApiController]
    [Authorize]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(IPackageService packageService, ILogger<PackagesController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetPackagesQueryDto query)
        {
            _logger.LogInformation("GET /packages - Fetching packages (Page: {Page}, PageSize: {PageSize})", query.Page, query.PageSize);

            var (packages, totalCount) = await _packageService.GetAllAsync(query.Page, query.PageSize);

            var result = packages.Select(p => new
            {
                p.Id,
                p.Name,
                cultureObjects = p.PackageCultureObjects.Select(pco => new
                {
                    id = pco.CultureObjectId,
                    availableVisits = pco.AvailableVisits
                }),
                p.ValidDays
            });

            return Ok(new
            {
                Data = result,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }
    }
}
