namespace InventoryDemo.Crosscutting
{
    public record UserDto(string Username, string Password, string Token);

    public record UserTableDto(int UserId, string Username, string Name, string Email);
}
