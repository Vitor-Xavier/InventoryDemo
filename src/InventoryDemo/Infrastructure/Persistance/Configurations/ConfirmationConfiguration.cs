using InventoryDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryDemo.Infrastructure.Persistance.Configurations
{
    public class ConfirmationConfiguration : IEntityTypeConfiguration<Confirmation>
    {
        public void Configure(EntityTypeBuilder<Confirmation> builder)
        {
            builder.Property(c => c.CodeVerifier)
                .HasMaxLength(128);
        }
    }
}
