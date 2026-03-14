using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Repository.Configuration
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>   
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();

            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ci => ci.Quantity)
                .IsRequired();
        }
    }
}

