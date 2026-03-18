using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Repository.Configuration
{
    public class AreaShippingFeeConfiguration : IEntityTypeConfiguration<AreaShippingFee>
    {
        public void Configure(EntityTypeBuilder<AreaShippingFee> builder)
        {
            builder.Property(x => x.Area)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Fee)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.Area)
                .IsUnique();
        }
    }
}
