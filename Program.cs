using Duende.Bff.Yarp;
using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
var builder = WebApplication.CreateBuilder(args);

 //default cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddBff()
    .AddRemoteApis() // This adds the capabilities needed to perform proxying to remote APIs.
    .AddServerSideSessions(); // Uses in-memory session store by default
    // .WithDefaultOpenIdConnectOptions(options =>
    // {
    //     options.Authority = "https://demo.duendesoftware.com";
    //     options.ClientId = "interactive.confidential";
    //     options.ClientSecret = "secret";
    //     options.ResponseType = "code";
    //     options.ResponseMode = "query";

    //     options.GetClaimsFromUserInfoEndpoint = true;
    //     options.SaveTokens = true;
    //     options.MapInboundClaims = false;

    //     options.Scope.Clear();
    //     options.Scope.Add("openid");
    //     options.Scope.Add("profile");

    //     // Add this scope if you want to receive refresh tokens
    //     options.Scope.Add("offline_access");
    // })
    // .WithDefaultCookieOptions(options =>
    // {
    //     // Because we use an identity server that's configured on a different site
    //     // (duendesoftware.com vs localhost), we need to configure the SameSite property to Lax.
    //     // Setting it to Strict would cause the authentication cookie not to be sent after logging in.
    //     // The user would have to refresh the page to get the cookie.
    //     // Recommendation: Set it to 'strict' if your IDP is on the same site as your BFF.
    //     options.Cookie.SameSite = SameSiteMode.Lax;
    // });

builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{
    options.ChallengeScheme = "schmeName";
    options.UseChallengeSchemeScopedTokens = false;

    options.ClientCredentialsScope = "api1 api2";
    options.ClientCredentialsResource = "urn:resource";
    options.ClientCredentialStyle = ClientCredentialStyle.PostBody;
});
builder.Services.AddAuthorization();
// registers HTTP client that uses the managed user access token
builder.Services.AddUserAccessTokenHttpClient("apiClient", configureClient: client =>
{
    client.BaseAddress = new Uri("https://remoteServer/");
});
// builder.Services.AddHttpClient(
//         AccessTokenManagementDefaults.BackChannelHttpClientName,
//         configureClient => {
//             // ...
//         });

var app = builder.Build();


app.UseAuthentication();
app.UseRouting();

// adds antiforgery protection for local APIs
app.UseBff();

// adds authorization for local and remote API endpoints
app.UseAuthorization();

// Place your custom routes after the 'UseAuthorization()'
app.MapGet("/hello-world", () => "hello-world")
    .AsBffApiEndpoint(); // This statement adds CSRF protection to the controller endpoints

// Map any call (including child routes) from /api/remote to https://remote-api-address
app.MapRemoteBffApiEndpoint("/api/remote", "https://remote-api-address")
    //.WithAccessToken(RequiredTokenType.Client);   

;

app.MapGet("/user-claims", (HttpContext httpContext) =>
{
    var principal = httpContext.User;

    // Get all claims from all identities
    var allClaims = principal.Claims.ToList();

    // Get specific identity
    var identities = principal.Identities.Select(identity => identity.AuthenticationType).ToList();

    return new
    {
        Claims = allClaims.Select(c => new { c.Type, c.Value }),
        Identities = identities
    };
});     
app.Run();

