using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using InterviewTask.Models;
using InterviewTask.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace InterviewTask.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthService _authService;
    private readonly IMemoryCache _memoryCache;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IAuthService authService,
        IMemoryCache memoryCache) : base(options, logger, encoder, clock)
    {
        _authService = authService;
        _memoryCache = memoryCache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return AuthenticateResult.Fail("Missing header Authorization");
        }

        var authHeader = authorizationHeader.ToString();
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Wrong way of authorization");
        }

        var encodedLoginPassword = authHeader["Basic ".Length..].Trim();
        var loginPassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedLoginPassword));

        var separatorIndex = loginPassword.IndexOf(':');
        if (separatorIndex == -1)
        {
            return AuthenticateResult.Fail("Wrong auth data");
        }

        var login = loginPassword[..separatorIndex];
        var password = loginPassword[(separatorIndex + 1)..];

        var authResult = await AuthenticateAsync(encodedLoginPassword, login, password);
        
        if (!authResult.Authenticated)
        {
            return AuthenticateResult.Fail("Wrong login or password");
        }

        var claims = new[] { new Claim(ClaimTypes.Name, login), new Claim("Id", authResult.Id.ToString()) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private async Task<AuthCredentials> AuthenticateAsync(string encodedLoginPassword, string login, string password)
    {
        AuthCredentials authResult;
        if (_memoryCache.TryGetValue($"auth-{encodedLoginPassword}", out AuthCredentials credentials))
        {
            authResult = credentials;
        }
        else
        {
            authResult = await _authService.Auth(login, password);
            _memoryCache.Set($"auth-{encodedLoginPassword}", authResult,
                absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(10));
        }

        return authResult;
    }
}