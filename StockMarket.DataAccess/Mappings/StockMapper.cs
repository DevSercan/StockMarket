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
    public class StockMapper : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.Property(s => s.Name).IsRequired().HasColumnType("varchar(50)");
            builder.Property(s => s.Price).IsRequired().HasColumnType("decimal(16,2)");
            builder.Property(s => s.Quantity).IsRequired();
            builder.Property(s => s.IsActive).IsRequired();

            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}
