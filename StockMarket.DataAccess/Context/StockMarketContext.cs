using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StockMarket.DataAccess.Mappings;
using StockMarket.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Transaction = StockMarket.Entities.Transaction;

namespace StockMarket.DataAccess.Context
{
    public class StockMarketContext : DbContext
    {
        public StockMarketContext(DbContextOptions<StockMarketContext> options)
            : base(options)
        {
        }
        public DbSet<BalanceCard> BalanceCards { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMapper());
            modelBuilder.ApplyConfiguration(new RoleMapper());
            modelBuilder.ApplyConfiguration(new StockMapper());
            modelBuilder.ApplyConfiguration(new CommissionMapper());
            modelBuilder.ApplyConfiguration(new PortfolioMapper());
            modelBuilder.ApplyConfiguration(new TransactionMapper());
            modelBuilder.ApplyConfiguration(new PriceHistoryMapper());
            modelBuilder.ApplyConfiguration(new BalanceCardMapper());
        }
    }
}
