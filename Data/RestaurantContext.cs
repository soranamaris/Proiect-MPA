using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proiect_MPA.Models;

namespace Proiect_MPA.Data
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext (DbContextOptions<RestaurantContext> options)
            : base(options)
        {
        }

        public DbSet<Proiect_MPA.Models.Zone>? Zone { get; set; } = default!;
        public DbSet<Proiect_MPA.Models.Waiter>? Waiter { get; set; } = default!;
        public DbSet<Proiect_MPA.Models.Schedule>? Schedule { get; set; } = default!;
        public DbSet<Proiect_MPA.Models.Client>? Client { get; set; } = default!;
        public DbSet<Proiect_MPA.Models.Table>? Table { get; set; }
        public DbSet<Proiect_MPA.Models.Schedule>? Schedules { get; set; }
        public DbSet<Proiect_MPA.Models.BookingSchedule>? BookingSchedule { get; set; }
        public DbSet<Proiect_MPA.Models.Reservation>? Reservation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table>()
                .HasOne(e => e.Reservation)
            .WithOne(e => e.Table)
                .HasForeignKey<Reservation>("ReservationID");
        }

    }
}
