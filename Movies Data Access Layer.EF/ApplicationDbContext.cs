using Microsoft.EntityFrameworkCore;
using Movies_Core_Layer.Models;

namespace Movies_Data_Access_Layer.EF;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
}
