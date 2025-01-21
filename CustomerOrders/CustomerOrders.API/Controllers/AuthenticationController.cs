using CustomerOrders.API.Interfaces;
using CustomerOrders.API.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CustomerOrders.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        readonly IAuthService authService;

        public Authentication(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// Authenticates the user based on provided credentials (username and password).
        /// </summary>
        /// <param name="username">The username of the user trying to log in.</param>
        /// <param name="password">The password of the user trying to log in.</param>
        /// <returns>An ActionResult containing the login response with the authentication result and user details, if successful.</returns>
        
        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponse>> LoginUserAsync([FromBody] UserLoginRequest request)
        {
            var result = await authService.LoginUserAsync(request);

            return result;
        }
     }
}