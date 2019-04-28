using Bit.Core.Models;
using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;

namespace ToDoLine.Security
{
    public class ToDoLineClientsProvider : OAuthClientsProvider
    {
        public virtual AppEnvironment AppEnvironment { get; set; }

        public override IEnumerable<Client> GetClients()
        {
            yield return GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
            {
                ClientId = "ToDoLine",
                ClientName = "ToDoLine",
                Enabled = true,
                Secret = "secret",
                TokensLifetime = TimeSpan.FromDays(7)
            });

            yield return GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
            {
                ClientId = "ToDoLineApp",
                ClientName = "ToDoLineApp",
                Enabled = true,
                Secret = "secret",
                TokensLifetime = TimeSpan.FromDays(7)
            });

            yield return GetImplicitFlowClient(new BitImplicitFlowClient
            {
                ClientName = "Test",
                ClientId = "Test",
                Secret = "secret",
                RedirectUris = new List<string>
                {
                    $@"^(http|https):\/\/(\S+\.)?(bit-framework.com|localhost|127.0.0.1|0f87b1dc.ngrok.io)(:\d+)?\b{AppEnvironment.GetHostVirtualPath()}\bSignIn\/?",
                    "Test://oauth2redirect"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    $@"^(http|https):\/\/(\S+\.)?(bit-framework.com|localhost|127.0.0.1|0f87b1dc.ngrok.io)(:\d+)?\b{AppEnvironment.GetHostVirtualPath()}\bSignOut\/?",
                    "Test://oauth2redirect"
                },
                TokensLifetime = TimeSpan.FromDays(7),
                Enabled = true
            });
        }
    }
}
