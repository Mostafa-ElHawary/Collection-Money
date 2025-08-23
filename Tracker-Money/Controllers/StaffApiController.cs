using Microsoft.AspNetCore.Mvc;
using CollectionApp.Application.Services;
using CollectionApp.Application.ViewModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Tracker_Money.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffApiController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffApiController> _logger;

        public StaffApiController(IStaffService staffService, ILogger<StaffApiController> logger)
        {
            _staffService = staffService;
            _logger = logger;
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<StaffListVM>>> GetActiveStaff()
        {
            try
            {
                var activeStaff = await _staffService.GetActiveStaffAsync();
                return Ok(activeStaff);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active staff");
                return StatusCode(500, "An error occurred while retrieving active staff");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffListVM>>> SearchStaff([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    var activeStaff = await _staffService.GetActiveStaffAsync();
                    return Ok(activeStaff);
                }

                var staff = await _staffService.SearchStaffAsync(searchTerm);
                return Ok(staff);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error searching staff");
                return StatusCode(500, "An error occurred while searching staff");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDetailVM>> GetStaffById(System.Guid id)
        {
            try
            {
                var staff = await _staffService.GetByIdAsync(id);
                return Ok(staff);
            }
            catch (System.InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Staff not found");
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff");
                return StatusCode(500, "An error occurred while retrieving staff");
            }
        }
    }
}