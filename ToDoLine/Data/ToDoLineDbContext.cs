using Bit.Core.Implementations;
using Bit.Core.Models;
using Bit.Data.EntityFrameworkCore.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Microsoft.EntityFrameworkCore;
using ToDoLine.Model;

namespace ToDoLine.Data
{
    public class ToDoLineDbContext : EfCoreDbContextBase
    {
        public ToDoLineDbContext()
            : base(new DbContextOptionsBuilder<ToDoLineDbContext>().UseSqlServer(DefaultAppEnvironmentsProvider.Current.GetActiveAppEnvironment().GetConfig<string>("AppConnectionString")).Options)
        {
        }

        public ToDoLineDbContext(AppEnvironment appEnv, IDbContextObjectsProvider dbContextCreationOptionsProvider)
            : base(appEnv.GetConfig<string>("AppConnectionString"), dbContextCreationOptionsProvider)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique(true);

            modelBuilder.Entity<ToDoGroupOptions>()
                .HasIndex(tdgo => new { tdgo.UserId, tdgo.ToDoGroupId })
                .IsUnique(true);

            modelBuilder.Entity<ToDoItemOptions>()
                .HasIndex(tdio => new { tdio.UserId, tdio.ToDoItemId })
                .IsUnique(true);

            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedToDoGroups)
                .WithOne(tdg => tdg.CreatedBy)
                .HasForeignKey(tdg => tdg.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CompletedToDoItems)
                .WithOne(tdi => tdi.CompletedBy)
                .HasForeignKey(tdg => tdg.CompletedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedToDoItems)
                .WithOne(tdi => tdi.CreatedBy)
                .HasForeignKey(tdg => tdg.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ToDoGroupOptionsList)
                .WithOne(tdgo => tdgo.User)
                .HasForeignKey(tdgo => tdgo.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ToDoItemOptionsList)
                .WithOne(tdi => tdi.User)
                .HasForeignKey(tdi => tdi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ToDoGroup>()
                .HasMany(tdg => tdg.Options)
                .WithOne(tdgo => tdgo.ToDoGroup)
                .HasForeignKey(tdgo => tdgo.ToDoGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoGroup>()
                .HasMany(tdg => tdg.Items)
                .WithOne(tdi => tdi.ToDoGroup)
                .HasForeignKey(tdi => tdi.ToDoGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoItem>()
                .HasMany(tdi => tdi.Steps)
                .WithOne(tdis => tdis.ToDoItem)
                .HasForeignKey(tdis => tdis.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoItem>()
                .HasMany(tdi => tdi.Options)
                .WithOne(tdio => tdio.ToDoItem)
                .HasForeignKey(tdoi => tdoi.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<ToDoGroup> ToDoGroups { get; set; }

        public virtual DbSet<ToDoGroupOptions> ToDoGroupOptionsList { get; set; }

        public virtual DbSet<ToDoItem> ToDoItems { get; set; }

        public virtual DbSet<ToDoItemOptions> ToDoItemOptionsList { get; set; }

        public virtual DbSet<ToDoItemStep> ToDoItemSteps { get; set; }
    }
}
