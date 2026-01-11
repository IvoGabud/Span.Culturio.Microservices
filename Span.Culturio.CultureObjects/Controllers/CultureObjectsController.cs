using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Span.Culturio.CultureObjects.Services.Interfaces;
using Span.Culturio.Shared.Models.DTOs;

namespace Span.Culturio.CultureObjects.Controllers
{
    [Route("culture-objects")]
    [ApiController]
    [Authorize]
    public class CultureObjectsController : ControllerBase
    {
        private readonly ICultureObjectService _cultureObjectService;
        private readonly ILogger<CultureObjectsController> _logger;

        public CultureObjectsController(ICultureObjectService cultureObjectService, ILogger<CultureObjectsController> logger)
        {
            _cultureObjectService = cultureObjectService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCultureObjectDto dto)
        {
            _logger.LogInformation("POST /culture-objects - Creating culture object");

            try
            {
                var cultureObject = await _cultureObjectService.CreateAsync(dto);
                return Ok(new { message = "Culture object created", id = cultureObject.Id });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create culture object: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /culture-objects - Fetching all culture objects");

            var cultureObjects = await _cultureObjectService.GetAllAsync();

            var result = cultureObjects.Select(co => new
            {
                co.Id,
                co.Name,
                co.ContactEmail,
                co.ZipCode,
                co.Address,
                co.City,
                co.AdminUserId
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /culture-objects/{Id} - Fetching culture object", id);

            var cultureObject = await _cultureObjectService.GetByIdAsync(id);

            if (cultureObject == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                cultureObject.Id,
                cultureObject.Name,
                cultureObject.ContactEmail,
                cultureObject.ZipCode,
                cultureObject.Address,
                cultureObject.City,
                cultureObject.AdminUserId
            });
        }
    }
}
