using FreshFalaye.Pos.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FreshFalaye.Pos.Shared.Data
{
    public class PosDbContext : DbContext
    {
        public PosDbContext(DbContextOptions<PosDbContext> options)
        : base(options)
        {
        }

        public DbSet<LocalSetting> LocalSettings => Set<LocalSetting>();
        public DbSet<LocalProduct> LocalProducts => Set<LocalProduct>();
        public DbSet<LocalStock> LocalStocks => Set<LocalStock>();
        public DbSet<LocalSale> LocalSales => Set<LocalSale>();
        public DbSet<LocalSaleItem> LocalSaleItems => Set<LocalSaleItem>();
        public DbSet<LocalSaleExpense> LocalSaleExpenses => Set<LocalSaleExpense>();
        public DbSet<LocalUser> LocalUsers => Set<LocalUser>();
        public DbSet<LocalProductGroup> LocalProductGroups => Set<LocalProductGroup>();
        public DbSet<LocalStoreSettings> LocalStoreSettings => Set<LocalStoreSettings>();
        public DbSet<LocalExpenseMaster> LocalExpenseMaster => Set<LocalExpenseMaster>();
        public DbSet<SyncState> SyncState => Set<SyncState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LocalSale>()
                .HasIndex(x => x.SyncId)
                .IsUnique();
        }
    }
}
