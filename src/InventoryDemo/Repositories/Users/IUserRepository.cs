using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default);

        Task<UserDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserTableDto>> GetUsers(int skip, int take, CancellationToken cancellationToken = default);

        Task<int> GetTotalUsers(CancellationToken cancellationToken = default);
    }
}
