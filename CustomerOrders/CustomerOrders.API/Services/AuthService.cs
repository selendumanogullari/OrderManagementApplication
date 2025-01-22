using CustomerOrders.API.Helpers;
using CustomerOrders.API.Interfaces;
using CustomerOrders.API.Models;
using CustomerOrders.API.Models.Login;
using CustomerOrders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrders.API.Services;

public class AuthService : IAuthService
{
    readonly ITokenService tokenService;
    private readonly DatabaseContext dbContext;

    public AuthService(ITokenService tokenService, DatabaseContext dbContext)
    {
        this.tokenService = tokenService;
        this.dbContext = dbContext;
    }

    public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request)
    {
        UserLoginResponse response = new();

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            throw new ArgumentNullException(nameof(request));
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            response.AuthenticateResult = false;
            return response;
        }

        bool isPasswordValid = SSHA256Helper.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            response.AuthenticateResult = false;
            return response;
        }

        var generatedTokenInformation = await tokenService.GenerateToken(new GenerateTokenRequest { Username = request.Username });

        response.AuthenticateResult = true;
        response.AuthToken = generatedTokenInformation.Token;
        response.AccessTokenExpireDate = generatedTokenInformation.TokenExpireDate;

        return response;
    }
}