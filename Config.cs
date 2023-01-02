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
            new IdentityResource("phone", "Phone number", new []{"phone"}),
            //new IdentityResource("email", "Email", new[]{"email"}),
            new IdentityResources.Email(),
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
            //new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
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
                        //IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "attendrapi.fullaccess",
                        "phone",
                    },


                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                }
            };
}