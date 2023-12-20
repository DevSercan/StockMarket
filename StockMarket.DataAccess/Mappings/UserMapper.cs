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
    public class UserMapper : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasColumnType("varchar(50)");
            builder.Property(u => u.LastName).IsRequired().HasColumnType("varchar(50)");
            builder.Property(u => u.Email).IsRequired().HasColumnType("varchar(100)");
            builder.Property(u => u.Password).IsRequired().HasColumnType("varchar(250)");
            builder.Property(u => u.RoleId).IsRequired();
            builder.Property(u => u.Balance).IsRequired().HasColumnType("decimal(16,2)");

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
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
        }
    }
}
