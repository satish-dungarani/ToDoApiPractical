using Microsoft.EntityFrameworkCore;
using ToDoApiPractical.Domains;

namespace ToDoApiPractical.DataContext
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Category> Categories { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TodoItem>()
                .Property(t => t.Priority)
                .HasDefaultValue(3);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);
        }
    }
}
