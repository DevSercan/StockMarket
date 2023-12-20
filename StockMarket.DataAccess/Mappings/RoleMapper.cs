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
    public class RoleMapper : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Name).IsRequired().HasColumnType("varchar(24)");
            builder.HasIndex(r => r.Name).IsUnique();

            builder.HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "user" },
                new Role { Id = 3, Name = "vault" }
            );
        }
    }
}
