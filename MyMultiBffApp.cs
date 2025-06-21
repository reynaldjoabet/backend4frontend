using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace backend4frontend;
public class MyMultiBffApp
{
   // private readonly IConfiguration? _configuration;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var cn = builder.Configuration.GetConnectionString("db");

        builder.Services.AddBff(options =>
        {   // default value
            options.ManagementBasePath = "/bff";
            options.EnableSessionCleanup = true;
            options.SessionCleanupInterval = TimeSpan.FromMinutes(5);
        })
         .AddEntityFrameworkServerSideSessions(options =>
            {
                options.UseSqlServer(cn);
            });
        // .WithDefaultOpenIdConnectOptions(options =>
        //         {
        //             options.Authority = "https://demo.duendesoftware.com";
        //             options.ClientId = "interactive.confidential";
        //             options.ClientSecret = "secret";
        //             options.ResponseType = "code";
        //             options.ResponseMode = "query";

        //             options.GetClaimsFromUserInfoEndpoint = true;
        //             options.SaveTokens = true;
        //             options.MapInboundClaims = false;

        //             options.Scope.Clear();
        //             options.Scope.Add("openid");
        //             options.Scope.Add("profile");

        //             // Add this scope if you want to receive refresh tokens
        //             options.Scope.Add("offline_access");
        //         })
        //         .WithDefaultCookieOptions(options =>
        //         {
        //             // Because we use an identity server that's configured on a different site
        //             // (duendesoftware.com vs localhost), we need to configure the SameSite property to Lax.
        //             // Setting it to Strict would cause the authentication cookie not to be sent after logging in.
        //             // The user would have to refresh the page to get the cookie.
        //             // Recommendation: Set it to 'strict' if your IDP is on the same site as your BFF.
        //             options.Cookie.SameSite = SameSiteMode.Lax;
        //         });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseRouting();

        // adds antiforgery protection for local APIs
        app.UseBff();

        // adds authorization for local and remote API endpoints
        app.UseAuthorization();

        app.MapBffManagementEndpoints();
        app.Run();
    }  
}