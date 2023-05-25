using InventoryDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace InventoryDemo.Infrastructure.Persistance.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(e => e.Title)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(256);

            builder.HasData(
                new Category { CategoryId = 1, Title = "Eletrônicos", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 2, Title = "Processador", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 3, Title = "Placa de vídeo", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 4, Title = "Memória", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 5, Title = "Fonte", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 6, Title = "Placa mãe", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 7, Title = "Armazenamento", ParentId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 8, Title = "SSD Sata", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 9, Title = "SSD M.2 Sata", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Category { CategoryId = 10, Title = "SSD PCIe", ParentId = 7, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
        }
    }
}
