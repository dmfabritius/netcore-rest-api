using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using netcore_rest_api.Models;

namespace netcore_rest_api.Data {

  public class ApiDbContext : IdentityDbContext {
    public virtual DbSet<ItemData> Items { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) {
    }
  }
}
