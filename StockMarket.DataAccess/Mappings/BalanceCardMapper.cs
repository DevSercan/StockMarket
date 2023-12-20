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
    public class BalanceCardMapper : IEntityTypeConfiguration<BalanceCard>
    {
        public void Configure(EntityTypeBuilder<BalanceCard> builder)
        {
            builder.Property(bc => bc.UserId).IsRequired();
            builder.Property(bc => bc.Code).IsRequired().HasColumnType("varchar(26)");
            builder.Property(bc => bc.Balance).IsRequired().HasColumnType("decimal(16,2)");
            builder.Property(bc => bc.IsUsed).IsRequired();

            builder.HasIndex(bc => bc.Code).IsUnique();

            builder.HasOne(bc => bc.User).WithMany(u => u.BalanceCards).HasForeignKey(bc => bc.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
