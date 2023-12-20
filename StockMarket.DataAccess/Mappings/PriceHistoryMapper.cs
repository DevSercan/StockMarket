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
    public class PriceHistoryMapper : IEntityTypeConfiguration<PriceHistory>
    {
        public void Configure(EntityTypeBuilder<PriceHistory> builder)
        {
            builder.Property(ph => ph.StockId).IsRequired();
            builder.Property(ph => ph.Date).IsRequired().HasColumnType("datetime");
            builder.Property(ph => ph.Price).IsRequired().HasColumnType("decimal(16,2)");

            builder.HasOne(ph => ph.Stock).WithMany(s => s.PriceHistories).HasForeignKey(ph => ph.StockId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
