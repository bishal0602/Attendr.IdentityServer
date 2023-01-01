using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Attendr.IdentityServer;

public static class Config
{

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("attendrapi", "Attendr API")
            {
                Scopes = { "attendrapi.fullaccess" },
                ApiSecrets = {new Secret("apisecret".Sha256())}, // TODO
                Description = "Gives full access to Attedr API",
                //UserClaims = new []{ }
            }
        };
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            {
            new ApiScope("attendrapi.fullaccess")
            };

    public static IEnumerable<Client> Clients =>
        new Client[]
            {
                new Client()
                {
                    ClientName = "Attendr App",
                    ClientId= "attendr",
                    ClientSecrets =
                    {
                        new Secret("clientsecret".Sha256()) // TODO
                    },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "attendrapi.fullaccess",
                    },

                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                }
            };
}