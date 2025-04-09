using System.Globalization;
using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var students = new List<Student>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reset the stream position for reading

                    if (fileExtension == ".csv")
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            bool isFirstRow = true;
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                if (isFirstRow)
                                {
                                    isFirstRow = false; // Skip the first row
                                    continue;
                                }
                                var values = line.Split(',');

                                // Assuming the data follows the provided CSV format
                                var student = new Student
                                {
                                    Class = values[0],
                                    Contact_No = double.TryParse(values[1], out var contactNo) ? contactNo : 0,
                                    Address = values[2],
                                    AdmissionNumber = values[3],
                                    DateOfBirth = DateTime.ParseExact(values[4], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                                    StudentName = values[5],
                                    FatherName = values[6],
                                    MotherName = values[7],
                                    AadhaarNumber = double.TryParse(values[8], out var aadhaarNo) ? aadhaarNo : 0,
                                    EmergencyContactNumber = double.TryParse(values[9], out var em_contact_no) ? em_contact_no : 0,
                                    SSSMID = double.TryParse(values[10], out var sssmid) ? sssmid : 0,
                                };
                                students.Add(student);
                            }
                        }
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var student = new Student
                                {
                                    Class = worksheet.Cells[row, 1].Text,
                                    Contact_No = double.TryParse(worksheet.Cells[row, 2].Text, out var contactNumber) ? contactNumber : 0,
                                    Address = worksheet.Cells[row, 3].Text,
                                    AdmissionNumber = worksheet.Cells[row, 4].Text,
                                    DateOfBirth = DateTime.ParseExact(worksheet.Cells[row, 5].Text, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                                    StudentName = worksheet.Cells[row, 6].Text,
                                    FatherName = worksheet.Cells[row, 7].Text,
                                    MotherName = worksheet.Cells[row, 8].Text,
                                    AadhaarNumber = double.TryParse(worksheet.Cells[row, 9].Text, out var aadhar) ? aadhar : 0,
                                    EmergencyContactNumber = double.TryParse(worksheet.Cells[row, 10].Text, out var result) ? result : 0,
                                    SSSMID = double.TryParse(worksheet.Cells[row, 11].Text, out var result1) ? result1 : 0,
                                };
                                students.Add(student);
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("Unsupported file type.");
                    }

                    await _context.Students.AddRangeAsync(students);
                    await _context.SaveChangesAsync();
                }

            }
             catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(UploadFile), new { fileName = file.FileName });
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == id))
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

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
