using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Data;
using Inventory_Management_System.Models.Category;

namespace Inventory_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly Inventory_Management_SystemContext _context;

        public CategoriesController(Inventory_Management_SystemContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryGetResponse>>> GetCategory()
        {
            var categories = await _context.Category.ToListAsync();
            var categoriesGetResponses = categories.Select(c => new CategoryGetResponse()
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            return Ok(categoriesGetResponses);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryGetResponse>> GetCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryGetResponse = new CategoryGetResponse()
            {
                Id = category.Id,
                Name = category.Name
            };

            return categoryGetResponse;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryPutResponse>> PutCategory(int id, CategoryPutRequest categoryPutRequest)
        {
            var category = _context.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryPutRequest.Name;

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var categoryPutResponse = new CategoryPutResponse()
            {
                Id = category.Id,
                Name = categoryPutRequest.Name
            };

            return Ok(categoryPutResponse);
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoryPostResponse>> PostCategory(CategoryPostRequest categoryPostRequest)
        {
            var category = new Category()
            {
                Name = categoryPostRequest.Name
            };

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            var categoryPostResponse = new CategoryPostResponse()
            {
                Id = category.Id,
                Name = categoryPostRequest.Name
            };

            return CreatedAtAction("GetCategory", new { id = categoryPostResponse.Id }, categoryPostResponse);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
