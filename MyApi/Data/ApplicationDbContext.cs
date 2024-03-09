using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
}