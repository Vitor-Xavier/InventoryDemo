using InventoryDemo.Crosscutting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Confirmations
{
    public interface IConfirmationService
    {
        Task<ConfirmationResponseDto> CreateConfirmation(string codeChallenge, CancellationToken cancellationToken = default);

        Task ValidateConfirmation(Guid code, string codeVerifier, string username, CancellationToken cancellationToken = default);
    }
}
