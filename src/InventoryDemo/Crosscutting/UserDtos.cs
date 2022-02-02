namespace InventoryDemo.Crosscutting
{
    public record UserDto(int UserId, string Username, string Name, string Email);

    public record UserAuthDto(string Username, string Password, string Token);

    public record UserTableDto(int UserId, string Username, string Name, string Email);
}
