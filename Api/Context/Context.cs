using Api.Entity;
using Microsoft.EntityFrameworkCore;

public class Context : DbContext
{   
    public Context(DbContextOptions<Context> options) : base(options) { }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}