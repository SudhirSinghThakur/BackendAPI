using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeConfigurationController : ControllerBase
    {
        private readonly SchoolContext _context;

        public FeeConfigurationController(SchoolContext context)
        {
            _context = context;
        }

        // POST: api/FeeManagement
        [HttpPost]
        public async Task<IActionResult> SaveFeeConfiguration([FromBody] FeeConfiguration feeConfiguration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Save the fee configuration to the database
                _context.FeeConfigurations.Add(feeConfiguration);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Fee structure saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the fee structure.", error = ex.Message });
            }
        }

        // GET: api/FeeManagement
        [HttpGet]
        public async Task<IActionResult> GetFeeConfigurations()
        {
            try
            {
                var feeConfigurations = await _context.FeeConfigurations
                    .ToListAsync();

                if (!feeConfigurations.Any())
                {
                    return NotFound(new { message = "No fee configurations found for the given class and academic year." });
                }

                return Ok(feeConfigurations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the fee configurations.", error = ex.Message });
            }
        }
    }
}
