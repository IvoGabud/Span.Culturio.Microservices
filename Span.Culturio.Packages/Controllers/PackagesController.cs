using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /packages - Fetching all packages");

            var packages = await _packageService.GetAllAsync();

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

            return Ok(result);
        }
    }
}
