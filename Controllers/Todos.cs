using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netcore_rest_api.Data;
using netcore_rest_api.Models;

namespace netcore_rest_api.Controllers {

  [ApiController]
  [Route("api/v1/[controller]")] // api/v1/Todos
  public class Todos : ControllerBase {

    private readonly ApiDbContext db;

    public Todos(ApiDbContext context) {
      db = context;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetItems() {
      var items = await db.Items.ToListAsync();
      return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id) {
      var item = await db.Items.FirstOrDefaultAsync(item => id == item.ItemID);
      if (item == null) return NotFound();
      return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(ItemData item) {
      if (!ModelState.IsValid) return new JsonResult("Server error") { StatusCode = 500 };
      await db.Items.AddAsync(item);
      await db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetItem), new { id = item.ItemID }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, ItemData item) {
      if (id != item.ItemID) return BadRequest();
      var existing = await db.Items.FirstOrDefaultAsync(item => id == item.ItemID);
      if (existing == null) return NotFound();
      existing.Title = item.Title;
      existing.Description = item.Description;
      existing.Done = item.Done;
      await db.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id) {
      var existing = await db.Items.FirstOrDefaultAsync(item => id == item.ItemID);
      if (existing == null) return NotFound();
      db.Items.Remove(existing);
      await db.SaveChangesAsync();
      return Ok(existing);
    }
  }
}