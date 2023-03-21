using Shared.Identity;

namespace Core.Identity
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(TokenRequest request);
    }
}
