using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using LoggingMicroservice.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
     IOptions<BasicAuthenticationOptions> _authOptions;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptions<BasicAuthenticationOptions> authOptions,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
         _authOptions = authOptions;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                Console.WriteLine(token);
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');

                var username = _authOptions.Value.Username; 
                var password = _authOptions.Value.Password; 

                if (credentials[0] == username && credentials[1] == password)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    };
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);

                    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
                }

                return AuthenticateResult.Fail("Invalid Username or Password");
            }
            else
            {
                return AuthenticateResult.Fail("Authentication Failed");
            }
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Authentication Failed: {ex.Message}");
        }
    }
}