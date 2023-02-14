using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Repositories.Users
{
    public class UserRepository : Repository<User, InventoryContext>, IUserRepository
    {
        public UserRepository(InventoryContext context) : base(context) { }

        public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username).AsNoTracking().SingleOrDefaultAsync(cancellationToken);

        public Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default) =>
            _context.Users.AsNoTracking().AnyAsync(user => user.Username == username, cancellationToken);

        public Task<UserAuthDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username && user.Password == password && !user.Deleted).AsNoTracking().Select(user => new UserAuthDto(user.Username, user.Password, null)).SingleOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default) =>
            await _context.Users.AsNoTracking().OrderBy(user => user.Name).Select(user => new UserDto(user.UserId, user.Username, user.Name, user.Email)).ToListAsync(cancellationToken);

        public async Task<IEnumerable<UserTableDto>> GetUsers(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Users.AsNoTracking().OrderBy(user => user.Name).Select(user => new UserTableDto(user.UserId, user.Username, user.Name, user.Email)).Skip(skip).Take(take).ToListAsync(cancellationToken);

        public Task<int> GetTotalUsers(CancellationToken cancellationToken = default) =>
            _context.Users.AsNoTracking().Where(user => !user.Deleted).CountAsync(cancellationToken);
    }
}
