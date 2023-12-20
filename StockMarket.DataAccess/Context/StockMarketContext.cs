using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasColumnType("varchar(50)");
                entity.Property(u => u.LastName).IsRequired().HasColumnType("varchar(50)");
                entity.Property(u => u.Email).IsRequired().HasColumnType("varchar(100)");
                entity.Property(u => u.Password).IsRequired().HasColumnType("varchar(250)");
                entity.Property(u => u.RoleId).IsRequired();
                entity.Property(u => u.Balance).IsRequired().HasColumnType("decimal(16,2)");

                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Name).IsRequired().HasColumnType("varchar(24)");

                entity.HasIndex(r => r.Name).IsUnique();
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(s => s.Name).IsRequired().HasColumnType("varchar(50)");
                entity.Property(s => s.Price).IsRequired().HasColumnType("decimal(16,2)");
                entity.Property(s => s.Quantity).IsRequired();
                entity.Property(s => s.IsActive).IsRequired();

                entity.HasIndex(s => s.Name).IsUnique();
            });

            modelBuilder.Entity<Portfolio>(entity =>
            {
                entity.Property(p => p.UserId).IsRequired();
                entity.Property(p => p.StockId).IsRequired();
                entity.Property(p => p.Quantity).IsRequired();

                entity.HasIndex(p => new { p.UserId, p.StockId }).IsUnique();

                entity.HasOne(p => p.User).WithMany(u => u.Portfolios).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(p => p.Stock).WithMany(s => s.Portfolios).HasForeignKey(p => p.StockId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.UserId).IsRequired();
                entity.Property(t => t.StockId).IsRequired();
                entity.Property(t => t.Type).IsRequired().HasColumnType("varchar(7)");
                entity.Property(t => t.Date).IsRequired().HasColumnType("datetime");
                entity.Property(t => t.Quantity).IsRequired();
                entity.Property(t => t.Price).IsRequired().HasColumnType("decimal(16,2)");
                entity.Property(t => t.Commission).IsRequired().HasColumnType("decimal(4,3)");

                entity.HasOne(t => t.User).WithMany(u => u.Transactions).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(t => t.Stock).WithMany(s => s.Transactions).HasForeignKey(t => t.StockId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.Property(ph => ph.StockId).IsRequired();
                entity.Property(ph => ph.Date).IsRequired().HasColumnType("datetime");
                entity.Property(ph => ph.Price).IsRequired().HasColumnType("decimal(16,2)");

                entity.HasOne(ph => ph.Stock).WithMany(s => s.PriceHistories).HasForeignKey(ph => ph.StockId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BalanceCard>(entity =>
            {
                entity.Property(bc => bc.UserId).IsRequired();
                entity.Property(bc => bc.Code).IsRequired().HasColumnType("varchar(26)");
                entity.Property(bc => bc.Balance).IsRequired().HasColumnType("decimal(16,2)");
                entity.Property(bc => bc.IsUsed).IsRequired();

                entity.HasIndex(bc => bc.Code).IsUnique();

                entity.HasOne(bc => bc.User).WithMany(u => u.BalanceCards).HasForeignKey(bc => bc.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Commission>(entity =>
            {
                entity.Property(c => c.CommissionRate).IsRequired().HasColumnType("decimal(4,3)");
            });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "admin",
                    Password = "admin",
                    RoleId = 1,
                    Balance = 0
                },
                new User
                {
                    Id = 2,
                    FirstName = "vault",
                    LastName = "vault",
                    Email = "vault",
                    Password = "vault",
                    RoleId = 3,
                    Balance = 0
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "user" },
                new Role { Id = 3, Name = "vault" }
            );
        }
    }
}
