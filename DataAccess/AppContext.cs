using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Car>().HasKey(car => car.Id);
            builder.Entity<Car>().Property(car => car.Text).HasMaxLength(250).IsRequired();
            builder.Entity<Car>().Property(car => car.RentalPricePerDay).HasPrecision(10, 2).IsRequired();
            builder.Entity<Car>().Property(car => car.IsAvailable).HasDefaultValue(true);
            builder.Entity<Car>().Property(car => car.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Entity<Car>().Property(car => car.EditDate).IsRequired(false);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<Car>();
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreationDate = now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.EditDate = now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
