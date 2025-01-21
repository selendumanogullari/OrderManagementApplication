using CustomerOrders.API.Models;
    
namespace CustomerOrders.API.Interfaces
{
    public interface ITokenService
    {
        public Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request);
    }
}
