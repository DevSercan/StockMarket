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
    public class PortfolioMapper : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.StockId).IsRequired();
            builder.Property(p => p.Quantity).IsRequired();

            builder.HasIndex(p => new { p.UserId, p.StockId }).IsUnique();

            builder.HasOne(p => p.User).WithMany(u => u.Portfolios).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Stock).WithMany(s => s.Portfolios).HasForeignKey(p => p.StockId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
