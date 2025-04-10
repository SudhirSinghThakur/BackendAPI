using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class ExamScheduleController : ControllerBase
{
    private readonly SchoolContext _context;

    public ExamScheduleController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/ExamSchedule
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExamSchedule>>> GetExamSchedules()
    {
        var examSchedules = await _context.ExamSchedules
          .Include(es => es.ExamDetails) // Include related ExamDetails
          .Select(es => new
          {
              Id = es.Id,
              Grade = es.Grade,
              ExamType = es.ExamType,
              Timetable = es.ExamDetails.Select(ed => new
              {
                  Subject = ed.Subject,
                  Date = ed.Date
              }).ToList()
          })
          .ToListAsync();

        return Ok(examSchedules);
    }

    // GET: api/ExamSchedule/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ExamSchedule>> GetExamSchedule(int id)
    {
        var examSchedule = await _context.ExamSchedules
        .Include(es => es.ExamDetails) // Include related ExamDetails
        .Where(es => es.Id == id)
        .Select(es => new
        {
            Id = es.Id,
            Grade = es.Grade,
            ExamType = es.ExamType,
            Timetable = es.ExamDetails.Select(ed => new
            {
                Subject = ed.Subject,
                Date = ed.Date
            }).ToList()
        })
        .FirstOrDefaultAsync();

        if (examSchedule == null)
        {
            return NotFound();
        }

        return Ok(examSchedule);
    }

    [HttpPost]
    public async Task<ActionResult<ExamSchedule>> PostExamSchedule([FromBody] ExamScheduleDto examScheduleDto)
    {
        if (examScheduleDto == null || string.IsNullOrEmpty(examScheduleDto.Grade) || string.IsNullOrEmpty(examScheduleDto.ExamType))
        {
            return BadRequest("Invalid data.");
        }

        var examSchedule = new ExamSchedule
        {
            Grade = examScheduleDto.Grade,
            ExamType = examScheduleDto.ExamType,
            ExamDetails = examScheduleDto.ExamDetails.Select(t => new ExamDetails
            {
                Subject = t.Subject,
                Date = t.Date
            }).ToList()
        };

        _context.ExamSchedules.Add(examSchedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExamSchedule), new { id = examSchedule.Id }, examSchedule);
    }

    // PUT: api/ExamSchedule/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExamSchedule(int id, [FromBody] ExamScheduleDto examScheduleDto)
    {
        if (examScheduleDto == null || id <= 0)
        {
            return BadRequest("Invalid data.");
        }

        var existingExamSchedule = await _context.ExamSchedules
            .Include(es => es.ExamDetails) // Include related ExamDetails
            .FirstOrDefaultAsync(es => es.Id == id);

        if (existingExamSchedule == null)
        {
            return NotFound("ExamSchedule not found.");
        }

        // Update the ExamSchedule fields
        existingExamSchedule.Grade = examScheduleDto.Grade;
        existingExamSchedule.ExamType = examScheduleDto.ExamType;
        existingExamSchedule.UpdatedAt = DateTime.Now;

        // Update the ExamDetails
        // Clear existing ExamDetails and add the new ones from the DTO
        _context.ExamDetails.RemoveRange(existingExamSchedule.ExamDetails);

        existingExamSchedule.ExamDetails = examScheduleDto.ExamDetails.Select(ed => new ExamDetails
        {
            Subject = ed.Subject,
            Date = ed.Date,
            ExamScheduleId = id // Ensure the relationship is maintained
        }).ToList();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExamScheduleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/ExamSchedule/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamSchedule(int id)
    {
        var examSchedule = await _context.ExamSchedules.FindAsync(id);
        if (examSchedule == null)
        {
            return NotFound();
        }

        _context.ExamSchedules.Remove(examSchedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExamScheduleExists(int id)
    {
        return _context.ExamSchedules.Any(e => e.Id == id);
    }
}

public class ExamScheduleDto
{
    public string Grade { get; set; }
    public string ExamType { get; set; }
    public List<ExamDetailDto> ExamDetails { get; set; }
}

public class ExamDetailDto
{
    public string Subject { get; set; }
    public DateTime Date { get; set; }
}
