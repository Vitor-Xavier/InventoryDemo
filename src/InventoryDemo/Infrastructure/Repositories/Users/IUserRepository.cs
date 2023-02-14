using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Repositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default);

        Task<UserAuthDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default);

        Task<IEnumerable<UserTableDto>> GetUsers(int skip, int take, CancellationToken cancellationToken = default);

        Task<int> GetTotalUsers(CancellationToken cancellationToken = default);
    }
}
