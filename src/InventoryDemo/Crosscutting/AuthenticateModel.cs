using System.ComponentModel.DataAnnotations;

namespace InventoryDemo.Crosscutting
{
    public record AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
