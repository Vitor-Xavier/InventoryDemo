using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Users
{
    public interface IUserService
    {
        ValueTask<User> GetUserById(int userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        ValueTask<UserDto> Authenticate(string username, string password, CancellationToken cancellationToken = default);

        Task CreateUser(User user, CancellationToken cancellationToken = default);

        Task UpdateUser(int userId, User user, CancellationToken cancellationToken = default);

        Task DeleteUser(int userId, CancellationToken cancellationToken = default);
    }
}
