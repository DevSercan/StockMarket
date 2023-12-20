using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Mappings
{
    public class TransactionMapper : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.StockId).IsRequired();
            builder.Property(t => t.Type).IsRequired().HasColumnType("varchar(7)");
            builder.Property(t => t.Date).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Quantity).IsRequired();
            builder.Property(t => t.Price).IsRequired().HasColumnType("decimal(16,2)");
            builder.Property(t => t.Commission).IsRequired().HasColumnType("decimal(4,3)");

            builder.HasOne(t => t.User).WithMany(u => u.Transactions).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Stock).WithMany(s => s.Transactions).HasForeignKey(t => t.StockId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
