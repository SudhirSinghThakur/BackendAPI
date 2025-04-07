using System.Security.Cryptography;
using System.Text;
using BackendAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Restrict access to Admin role
public class AdminController : ControllerBase
{
    private readonly SchoolContext _context;

    public AdminController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // POST: api/admin/users
    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] User newUser)
    {
        if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
        {
            return BadRequest("User already exists.");
        }

        newUser.PasswordHash = HashPassword(newUser.Username); // Hash the password before saving
        _context.Users.Add(new Models.User() { Role = newUser.Role, Username = newUser.Username });
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // GET: api/admin/users/{id}
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }

    // PUT: api/admin/users/{id}
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Username = updatedUser.Username;
        user.Role = updatedUser.Role;
        if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
        {
            user.PasswordHash = HashPassword(updatedUser.PasswordHash); // Hash the new password if provided
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/admin/users/{id}
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
