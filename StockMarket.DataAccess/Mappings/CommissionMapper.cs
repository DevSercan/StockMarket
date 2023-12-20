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
    public class CommissionMapper : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            builder.Property(c => c.CommissionRate).IsRequired().HasColumnType("decimal(4,3)");

            builder.HasData(
                new Commission { Id = 1, CommissionRate = 0.05m }
            );
        }
    }
}
