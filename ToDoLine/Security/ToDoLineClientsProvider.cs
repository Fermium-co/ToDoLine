using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;

namespace ToDoLine.Security
{
    public class ToDoLineClientsProvider : OAuthClientsProvider
    {
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
        }
    }
}
