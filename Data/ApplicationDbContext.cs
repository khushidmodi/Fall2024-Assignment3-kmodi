using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_kmodi.Models;

namespace Fall2024_Assignment3_kmodi.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Fall2024_Assignment3_kmodi.Models.Actor> Actor { get; set; } = default!;

    public DbSet<Fall2024_Assignment3_kmodi.Models.Movie> Movie { get; set; } = default!;

    public DbSet<Fall2024_Assignment3_kmodi.Models.MovieActor> MovieActor { get; set; } = default!;


}

