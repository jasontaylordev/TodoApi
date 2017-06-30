using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly TodoDbContext _context;

        public ItemsController(TodoDbContext context)
        {
            _context = context;
        }

        // GET api/items
        [HttpGet]
        public async Task<IEnumerable<Item>> GetAll()
        {
            return await _context.Items.ToListAsync();
        }

        // GET api/items/5
        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        // POST api/items
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Item item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetTodo", new { item.Id }, item);
        }

        // PUT api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]Item item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _context.Items.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Description = item.Description;
            entity.Done = item.Done;

            _context.Items.Update(entity);

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Items.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.Items.Remove(entity);

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
