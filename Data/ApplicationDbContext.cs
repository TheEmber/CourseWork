using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourseWork.Models;

namespace CourseWork.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.Seat);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Flight)
            .WithMany(f => f.Tickets)
            .HasForeignKey(t => t.FlightID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.BookedBy)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Flight>()
            .HasKey(f => f.ID);

        modelBuilder.Entity<User>()
            .HasKey(u => u.ID);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Role>()
            .HasKey(r => r.ID);
    }
    public static void SeedData(IServiceProvider serviceProvider)
        {
        using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        // Check if data already exists
        if (context.Flights.Any() || context.Users.Any() || context.Tickets.Any() || context.Roles.Any())
        {
            return; // Data already seeded
        }

        // Add sample flights
        var flights = new[]
        {
                    new Flight
                    {
                        ID = Guid.NewGuid(),
                        Source = "City A",
                        Destination = "City B",
                        DepartureDate = DateTime.Now.AddDays(1),
                        ArrivalDate = DateTime.Now.AddDays(1).AddHours(3),
                        Price = 150.00
                    },
                    new Flight
                    {
                        ID = Guid.NewGuid(),
                        Source = "City B",
                        Destination = "City C",
                        DepartureDate = DateTime.Now.AddDays(2),
                        ArrivalDate = DateTime.Now.AddDays(2).AddHours(2),
                        Price = 200.00
                    },
                    // Add more flights as needed
                };

        context.Flights.AddRange(flights);
        context.SaveChanges();

        // Add sample roles
        var roles = new[]
        {
                    new Role
                    {
                        Name = "User"
                    },
                    new Role
                    {
                        Name = "Manager"
                    },
                    new Role
                    {
                        Name = "Admin"
                    }
                };

        context.Roles.AddRange(roles);
        context.SaveChanges();

        // Add sample users
        var userRole = context.Roles.Single(r => r.Name == "User");
        var adminRole = context.Roles.Single(r => r.Name == "Admin");
        var users = new[]
        {
                    User.RegisterUser("admin@admin.admin", "admin", "admin", "admin", adminRole),
                    User.RegisterUser("user2@example.com", "password2", "name2", "surname2", userRole)
                };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Add sample tickets
        var tickets = new[]
        {
                    new Ticket
                    {
                        Seat = 1,
                        FlightID = flights[0].ID,
                        BookedBy = null
                    },
                    new Ticket
                    {
                        Seat = 2,
                        FlightID = flights[1].ID,
                        BookedBy = null
                    },
                    // Add more tickets as needed
                };

        context.Tickets.AddRange(tickets);
        context.SaveChanges();
    }
}