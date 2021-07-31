using InventoryDemo.Services.Strategies;

namespace InventoryDemo.Services.Factories
{
    public class OrderFormatFactory : IOrderFormatFactory
    {
        public IOrderFormatStrategy[] Create() => new IOrderFormatStrategy[]
        {
            new CsvFormatStrategy(),
            new JsonFormatStrategy()
        };
    }
}
