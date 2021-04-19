namespace InventoryDemo.Crosscutting
{
    public record AppSettings
    {
        public string Secret { get; set; }

        public int ExpiresIn { get; set; }
    }
}
