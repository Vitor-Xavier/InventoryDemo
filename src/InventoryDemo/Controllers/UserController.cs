using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) =>
            _userService = userService;

        /// <summary>
        /// Busca paginada de Usuários.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Usuários</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var suppliers = await _userService.GetUsers(skip, take, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Busca usuário por sua identificação.
        /// </summary>
        /// <param name="userId">Identificação do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Usuário</returns>
        [HttpGet("{userId:int}")]
        public ValueTask<User> FindUserById(int userId, CancellationToken cancellationToken) =>
            _userService.GetUserById(userId, cancellationToken);

        /// <summary>
        /// Autentica usuário com base em suas credenciais.
        /// </summary>
        /// <param name="authenticateModel">Credenciais do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Usuário</returns>
        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public ValueTask<UserAuthDto> Authenticate(AuthenticateModel authenticateModel, CancellationToken cancellationToken) =>
            _userService.Authenticate(authenticateModel.Username, authenticateModel.Password, cancellationToken);

        /// <summary>
        /// Busca por usuário através do nome de usuário.
        /// </summary>
        /// <param name="username">Nome de usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Usuário</returns>
        [HttpGet("{username}")]
        public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken) =>
            _userService.GetUserByUsername(username, cancellationToken);

        /// <summary>
        /// Insere usuário.
        /// </summary>
        /// <param name="user">Dados do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> PostUser(User user, CancellationToken cancellationToken)
        {
            await _userService.CreateUser(user, cancellationToken);
            return Created(nameof(User), user);
        }

        /// <summary>
        /// Alterar dados de usuário.
        /// </summary>
        /// <param name="userId">Identificação do usuário</param>
        /// <param name="user">Dados do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Updated user</returns>
        [HttpPut("{userId:int}")]
        public async Task<ActionResult<User>> PutUser(int userId, User user, CancellationToken cancellationToken)
        {
            await _userService.UpdateUser(userId, user, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Remove usuário.
        /// </summary>
        /// <param name="userId">Identificação do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{userId:int}")]
        public async Task<ActionResult> DeleteUser(int userId, CancellationToken cancellationToken)
        {
            await _userService.DeleteUser(userId, cancellationToken);
            return NoContent();
        }
    }
}
