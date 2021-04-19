using InventoryDemo.Context;
using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Users
{
    public class UserRepository : Repository<User, InventoryContext>, IUserRepository
    {
        public UserRepository(InventoryContext context) : base(context) { }

        public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username).AsNoTracking().SingleOrDefaultAsync(cancellationToken);

        public Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default) =>
            _context.Users.AsNoTracking().AnyAsync(user => user.Username == username, cancellationToken);

        public Task<UserDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username && user.Password == password && !user.Deleted).AsNoTracking().Select(user => new UserDto(user.Username, user.Password, null)).SingleOrDefaultAsync(cancellationToken);
    }
}
