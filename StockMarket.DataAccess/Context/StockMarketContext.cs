using Microsoft.EntityFrameworkCore;
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

        }
    }
}
