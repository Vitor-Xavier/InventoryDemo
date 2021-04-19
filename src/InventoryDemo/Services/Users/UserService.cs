using InventoryDemo.Crosscutting;
using InventoryDemo.Helpers;
using InventoryDemo.Models;
using InventoryDemo.Repositories.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;

        private readonly AppSettings _appSettings;

        private readonly IUserRepository _userRepository;

        public UserService(ILogger<UserService> logger, IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public ValueTask<User> GetUserById(int userId, CancellationToken cancellationToken = default) =>
            _userRepository.GetById(userId, cancellationToken);

        public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default) =>
            await _userRepository.GetUserByUsername(username, cancellationToken);

        public async ValueTask<UserDto> Authenticate(string username, string password, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await _userRepository.GetUserByUsernamePassword(username, password, cancellationToken) is UserDto user)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.Username.ToString()) }),
                    Expires = DateTime.UtcNow.AddMinutes(_appSettings.ExpiresIn),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return user with { Password = null, Token = tokenHandler.WriteToken(token) };
            }
            _logger.LogInformation($"Authentication failed for user '{username}' at {DateTime.Now}");
            throw new Exception("Username or password is incorrect");
        }

        public async Task CreateUser(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await IsValid(user, cancellationToken)) throw new Exception("Registro inválido");

            user.Password = EncodingHelper.ComputeSha256Hash(user.Password);

            await _userRepository.Add(user, cancellationToken);
        }

        public async Task UpdateUser(int userId, User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await IsValid(user, cancellationToken)) throw new Exception("Registro inválido");

            user.UserId = userId;
            user.Password = EncodingHelper.ComputeSha256Hash(user.Password);

            await _userRepository.Edit(user, cancellationToken);
        }

        public async Task DeleteUser(int userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Models.User user = new() { UserId = userId, Deleted = true };

            await _userRepository.Delete(user, cancellationToken);
        }

        public async Task<bool> IsValid(User user, CancellationToken cancellationToken = default) =>
            user is { Username: { Length: > 0 }, Password: { Length: > 0 }, Email: { Length: > 0 }, Name: { Length: > 0 } } &&
            (user.UserId is not 0 || !await _userRepository.UsernameIsDefined(user.Username, cancellationToken));
    }
}
