using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Identity.Identity.Constants;
using IdentityModel;

namespace Identity.Configurations;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new(Constants.StandardScopes.ChatApi),
            new(Constants.StandardScopes.ImageApi),
            new(Constants.StandardScopes.AssistantApi),
            new(Constants.StandardScopes.MeetingApi),
            new(Constants.StandardScopes.PaymentApi),
            new(Constants.StandardScopes.ResumeApi),
            new(Constants.StandardScopes.UserApi),
            new(Constants.StandardScopes.IdentityApi),
            new(Constants.StandardScopes.AIModularMonolith),
            new(JwtClaimTypes.Role, new List<string> {"role"})
        };


    public static IList<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new(Constants.StandardScopes.ChatApi)
            {
                Scopes = { Constants.StandardScopes.ChatApi }
            },
            new(Constants.StandardScopes.ImageApi)
            {
                Scopes = { Constants.StandardScopes.ImageApi }
            },
            new(Constants.StandardScopes.AssistantApi)
            {
                Scopes = { Constants.StandardScopes.AssistantApi }
            },
            new(Constants.StandardScopes.MeetingApi)
            {
                Scopes = { Constants.StandardScopes.MeetingApi }
            },
            new(Constants.StandardScopes.PaymentApi)
            {
                Scopes = { Constants.StandardScopes.PaymentApi }
            },
            new(Constants.StandardScopes.ResumeApi)
            {
                Scopes = { Constants.StandardScopes.ResumeApi }
            },
            new(Constants.StandardScopes.UserApi)
            {
                Scopes = { Constants.StandardScopes.UserApi }
            },
            new(Constants.StandardScopes.IdentityApi)
            {
                Scopes = { Constants.StandardScopes.IdentityApi }
            },
            new(Constants.StandardScopes.AIModularMonolith)
            {
                Scopes = { Constants.StandardScopes.AIModularMonolith }
            },
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    JwtClaimTypes.Role, // Include roles scope
                    Constants.StandardScopes.ChatApi,
                    Constants.StandardScopes.PaymentApi,
                    Constants.StandardScopes.MeetingApi,
                    Constants.StandardScopes.AssistantApi,
                    Constants.StandardScopes.ResumeApi,
                    Constants.StandardScopes.ImageApi,
                    Constants.StandardScopes.UserApi,
                    Constants.StandardScopes.IdentityApi,
                    Constants.StandardScopes.AIModularMonolith,
                },
                AccessTokenLifetime = 3600,  // authorize the client to access protected resources
                IdentityTokenLifetime = 3600, // authenticate the user,
                AlwaysIncludeUserClaimsInIdToken = true // Include claims in ID token
            }
        };
}