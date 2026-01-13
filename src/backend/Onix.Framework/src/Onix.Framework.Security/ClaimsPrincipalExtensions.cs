using System;
using System.Security.Claims;

namespace Onix.Framework.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal? user, out Guid usuarioId)
        {
            usuarioId = default;

            if (user == null)
            {
                return false;
            }

            var usuarioIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            {
                return false;
            }

            return Guid.TryParse(usuarioIdClaim, out usuarioId);
        }
    }
}
