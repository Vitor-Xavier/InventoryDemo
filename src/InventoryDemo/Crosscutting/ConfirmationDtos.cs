using System;

namespace InventoryDemo.Crosscutting
{
    public record ConfirmationResponseDto(Guid Code, DateTime ExpiresAt);
}
