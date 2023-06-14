using Dogs_API.Data;
using Dogs_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogs_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DogsController : ControllerBase
    {
        private readonly DogsContext _context;

        public DogsController(DogsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDogs(string attribute = null, string order = null, int pageNumber = 1, int limit = 10)
        {
            var query = _context.Dogs.AsQueryable();

            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                var propertyInfo = typeof(Dog).GetProperty(attribute);
                if (propertyInfo != null)
                {
                    query = order.ToLower() == "desc" ? query.OrderByDescending(d => EF.Property<object>(d, attribute)) : query.OrderBy(d => EF.Property<object>(d, attribute));
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

            if (pageNumber < 1)
                pageNumber = 1;
            else if (pageNumber > totalPages)
                pageNumber = totalPages;

            var dogs = await query.Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                Limit = limit,
                Dogs = dogs
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDog(Dog dog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Dogs.Any(d => d.Name == dog.Name))
            {
                return Conflict("A dog with the same name already exists.");
            }

            try
            {
                _context.Dogs.Add(dog);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDogs), new { id = dog.Id }, dog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the dog: {ex.Message}");
            }
        }
    }
}
