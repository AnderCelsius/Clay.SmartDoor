using Clay.SmartDoor.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clay.SmartDoor.Infrastructure.Data
{
    public class SmartDoorContext : IdentityDbContext<AppUser>
    {
        public SmartDoorContext(DbContextOptions<SmartDoorContext> options)
            : base(options) { }

        public DbSet<Door> Doors { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<AccessGroup> AccessGroups { get; set; }
        public DbSet<DoorAssignment> DoorAssignment { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.LastModified = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region ActivityLog Configuration
            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(u => u.ActionBy)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityLog>()
                .HasIndex(al => al.ActionBy)
                .IsUnique(false);

            modelBuilder.Entity<ActivityLog>()
                .Property(d => d.Time)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ActivityLog>()
                .Property(d => d.ActionBy)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ActivityLog>()
                .Property(d => d.Description)
                .HasMaxLength(250)
                .IsRequired();

            modelBuilder.Entity<ActivityLog>()
                .Property(d => d.Building)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ActivityLog>()
                .Property(d => d.Floor)
                .HasMaxLength(100)
                .IsRequired();
            #endregion

            #region Door Configuration
            modelBuilder.Entity<Door>()
                .HasIndex(d => d.NameTag)
                .IsUnique(false);

            modelBuilder.Entity<Door>()
                .Property(d => d.NameTag)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Door>()
                .Property(d => d.Building)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Door>()
                .Property(d => d.Floor)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Door>()
                .Property(d => d.CreatedAt)
                .HasColumnName("Date_Created")
                .IsRequired();

            modelBuilder.Entity<Door>()
                .Property(d => d.LastModified)
                .HasColumnName("Last_Modified")
                .IsRequired();

            modelBuilder.Entity<Door>()
                .Property(d => d.CreatorBy)
                .HasColumnName("Creator_Id")
                .IsRequired();
            #endregion

            #region User Configuration
            modelBuilder.Entity<AppUser>()
                .Property(au => au.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AppUser>()
                .Property(au => au.LastName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AppUser>()
                .Property(au => au.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<AppUser>()
                .Property(au => au.LastModified)
                .IsRequired();

            modelBuilder.Entity<AppUser>()
                .Property(au => au.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();
            #endregion

            #region AccessGroups Configuration
            modelBuilder.Entity<AccessGroup>()
                .HasKey(ag => ag.Id);

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.IsActive)
                .IsRequired();

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.LastModified)
                .IsRequired();

            modelBuilder.Entity<AccessGroup>()
                .Property(ag => ag.CreatorBy)
                .IsRequired();
            #endregion

            #region DoorAssignment Configuration
            modelBuilder.Entity<DoorAssignment>()
                .HasKey(da => da.Id);
            
            modelBuilder.Entity<DoorAssignment>()
                .Property(da => da.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<DoorAssignment>()
                .Property(da => da.DoorId)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<DoorAssignment>()
                .Property(da => da.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<DoorAssignment>()
                .Property(da => da.LastModified)
                .IsRequired();

            modelBuilder.Entity<DoorAssignment>()
                .Property(da => da.CreatorBy)
                .IsRequired();

            modelBuilder.Entity<DoorAssignment>()
                .HasOne(da => da.AccessGroup)
                .WithMany(da => da.DoorAssignment)
                .HasForeignKey(da => da.AccessGroupId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoorAssignment>()
                .HasIndex(da => da.AccessGroupId);

            modelBuilder.Entity<DoorAssignment>()
                .HasIndex(da => da.DoorId);
            #endregion

        }
    }
}
