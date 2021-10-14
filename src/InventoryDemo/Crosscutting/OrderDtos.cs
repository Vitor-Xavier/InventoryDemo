using System;
using System.Collections.Generic;

namespace InventoryDemo.Crosscutting
{
    public record OrderTableDto(int OrderId, DateTime Date, string Note);

    public record OrderDto(int OrderId, DateTime Date, string Note);

    public record OrderExportDto
    {
        public int OrderId { get; set; }

        public DateTime Date { get; set; }

        public string Note { get; set; }

        public IEnumerable<ProductOrderDto> Products { get; set; }
    }

    public record OrderImportDto
    {
        public int OrderId { get; set; }

        public DateTime Date { get; set; }

        public string Note { get; set; }

        public IEnumerable<ProductOrderDto> Products { get; set; }
    }
}
