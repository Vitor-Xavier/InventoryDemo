using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class Category : BaseEntity
    {
        public int CategoryId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        [JsonIgnore]
        public Category Parent { get; set; }

        [JsonIgnore]
        public ICollection<Category> Subcategories { get; set; } = new HashSet<Category>();

        public override bool Equals(object obj) => obj is Category category && category.CategoryId == CategoryId;

        public override int GetHashCode() => HashCode.Combine(CategoryId);
    }
}
