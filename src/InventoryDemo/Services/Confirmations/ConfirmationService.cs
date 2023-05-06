using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Repositories.Confirmations;
using InventoryDemo.Infrastructure.Persistance.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Confirmations
{
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _accessor;

        public ConfirmationService(IConfirmationRepository confirmationRepository,
                                   IUserRepository userRepository,
                                   IHttpContextAccessor accessor)
        {
            _confirmationRepository = confirmationRepository;
            _userRepository = userRepository;
            _accessor = accessor;
        }

        public async Task<ConfirmationResponseDto> CreateConfirmation(string codeChallenge, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, cancellationToken) ?? throw new BadHttpRequestException("Usuário inválido");

            string codeVerifier = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(codeChallenge)));

            var confirmation = new Confirmation { CodeVerifier = codeVerifier, ExpiresAt = DateTime.Now.AddMinutes(10), State = ConfirmationStatus.AwaitingConfirmation, UserId = user.UserId };
            await _confirmationRepository.Add(confirmation, cancellationToken);

            return new ConfirmationResponseDto(confirmation.ConfirmationId, confirmation.ExpiresAt);
        }

        public async Task ValidateConfirmation(Guid code, string codeVerifier, string username, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var confirmation = await _confirmationRepository.GetConfirmation(code, cancellationToken) ?? throw new BadHttpRequestException("Código de Confirmação inválido");
            var user = await _userRepository.GetUserByUsername(username, cancellationToken) ?? throw new BadHttpRequestException("Usuário inválido");

            if (confirmation.UserId != user.UserId)
                throw new BadHttpRequestException("Código de Confirmação emitido a outro usuário");

            if (confirmation.ExpiresAt > DateTime.Now)
                throw new BadHttpRequestException("Código de Confirmação expirado");

            if (confirmation.State == ConfirmationStatus.Confirmed)
                throw new BadHttpRequestException("Código de Confirmação já utilizado");

            if (confirmation.CodeVerifier == codeVerifier)
                throw new BadHttpRequestException("Código de Verificação inválido");

            confirmation.State = ConfirmationStatus.Confirmed;
            await _confirmationRepository.Edit(confirmation, cancellationToken);
        }
    }
}
