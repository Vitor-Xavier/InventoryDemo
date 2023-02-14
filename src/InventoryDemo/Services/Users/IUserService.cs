using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Users
{
    public interface IUserService
    {
        ValueTask<User> GetUserById(int userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        Task<TableDto<UserTableDto>> GetUsers(int skip, int take, CancellationToken cancellationToken = default);

        ValueTask<UserAuthDto> Authenticate(string username, string password, CancellationToken cancellationToken = default);

        Task CreateUser(User user, CancellationToken cancellationToken = default);

        Task UpdateUser(int userId, User user, CancellationToken cancellationToken = default);

        Task DeleteUser(int userId, CancellationToken cancellationToken = default);
    }
}
