using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourseWork.Models;

namespace CourseWork.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
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
    }
    public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if data already exists
                if (context.Flights.Any() || context.Users.Any() || context.Tickets.Any())
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

                // Add sample users
                var users = new[]
                {
                    new User
                    {
                        ID = Guid.NewGuid(),
                        Email = "user1@example.com",
                        Password = "password1",
                        Name = "John",
                        Surname = "Doe"
                    },
                    new User
                    {
                        ID = Guid.NewGuid(),
                        Email = "user2@example.com",
                        Password = "password2",
                        Name = "Jane",
                        Surname = "Smith"
                    },
                    // Add more users as needed
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
                        BookedBy = users[0].ID
                    },
                    new Ticket
                    {
                        Seat = 2,
                        FlightID = flights[1].ID,
                        BookedBy = users[1].ID
                    },
                    // Add more tickets as needed
                };

                context.Tickets.AddRange(tickets);
                context.SaveChanges();
            }
        }
}