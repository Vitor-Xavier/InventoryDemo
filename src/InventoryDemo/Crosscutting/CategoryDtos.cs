using System.Collections.Generic;

namespace InventoryDemo.Crosscutting
{
    public record CategoryDto(string Title, string Description);

    public record CategoryTableDto(int CategoryId, string Title, string Description);

    public record SubcategoryDto
    {
        public int CategoryId { get; set; }

        public string Title { get; set; }

        public IEnumerable<SubcategoryDto> Subcategories { get; set; } = new HashSet<SubcategoryDto>();
    }
}
