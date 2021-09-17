namespace InventoryDemo.Crosscutting
{
    public record ProductTableDto(int ProductId, string Name, string Code, string Description, decimal PricePerUnit, decimal MinimumRequired);

    public record ProductDto(int ProductId, string Name, string Code, string Description, decimal PricePerUnit, decimal MinimumRequired);

    public record ProductOrderDto(int ProductId, string Name, string Code, decimal Quantity, decimal PricePerUnit, decimal TotalPrice);
}
