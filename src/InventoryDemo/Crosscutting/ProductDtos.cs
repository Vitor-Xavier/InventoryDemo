namespace InventoryDemo.Crosscutting
{
    public record ProductTableDto(int ProductId, string Name, string Description, decimal PricePerUnit);

    public record ProductDto(int ProductId, string Name, string Code, string Description, decimal PricePerUnit, decimal MinimumRequired);
}
