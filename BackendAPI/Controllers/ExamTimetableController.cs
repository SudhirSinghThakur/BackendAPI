using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
        return await _context.ExamSchedules.ToListAsync();
    }

    // GET: api/ExamSchedule/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ExamSchedule>> GetExamSchedule(int id)
    {
        var examSchedule = await _context.ExamSchedules.FindAsync(id);

        if (examSchedule == null)
        {
            return NotFound();
        }

        return examSchedule;
    }

    //// POST: api/ExamSchedule
    //[HttpPost]
    //public async Task<ActionResult<ExamSchedule>> PostExamSchedule([FromBody] ExamSchedule examSchedule)
    //{
    //    _context.ExamSchedules.Add(examSchedule);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction(nameof(GetExamSchedule), new { id = examSchedule.Id }, examSchedule);
    //}

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
            ExamDetails = examScheduleDto.Timetable.Select(t => new ExamDetails
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
    public async Task<IActionResult> PutExamSchedule(int id, ExamSchedule examSchedule)
    {
        if (id != examSchedule.Id)
        {
            return BadRequest();
        }

        examSchedule.UpdatedAt = DateTime.Now;
        _context.Entry(examSchedule).State = EntityState.Modified;

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
    public List<ExamDetailDto> Timetable { get; set; }
}

public class ExamDetailDto
{
    public string Subject { get; set; }
    public DateTime Date { get; set; }
}
