using CustomerOrders.API.Models.Login;

namespace CustomerOrders.API.Interfaces
{
    public interface IAuthService
    {
        public Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request);
    }
}
