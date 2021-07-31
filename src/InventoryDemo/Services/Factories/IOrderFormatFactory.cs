using InventoryDemo.Services.Strategies;

namespace InventoryDemo.Services.Factories
{
    public interface IOrderFormatFactory
    {
        IOrderFormatStrategy[] Create();
    }
}
