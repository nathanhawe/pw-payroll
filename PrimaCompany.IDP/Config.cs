// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace PrimaCompany.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { 
                new ApiResource(
                    "timeandattendanceapi",
                    "Time and Attendance API")
                {
                    ApiSecrets = { new Secret("apisecret".Sha256()) }
                }
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            { 
                new Client
                {
                    ClientId = "timeandattendance",
                    ClientName = "Time and Attendance",
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Jwt,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "timeandattendanceapi"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = new List<string>()
                    {
                        "http://localhost:3000/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "http://localhost:3000/signout-callback-oidc"
                    }
                    ,AllowedCorsOrigins = new List<string>()
					{
                        "http://localhost:3000"
					}
                }
            };
        
    }
}