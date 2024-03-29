﻿using Bit.Http.Contracts;
using IdentityModel.Client;
using Simple.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLine.Test.Controller
{
    public class ToDoLineClient
    {
        /// <summary>
        /// Every request you make using this odata client, uses <see cref="Token"/>
        /// </summary>
        public IODataClient ODataClient { get; set; }

        public Token Token { get; set; }

        /// <summary>
        /// In these tests, user names are random guid values, you might need the user name, because it's not a well knwon hard coded user name.
        /// </summary>
        public string UserName { get; set; }
    }

    public static class ClientOperations
    {
        /// <summary>
        /// Registers new user. If userName gets provided, it uses that for registration. Otherwise, it creates random user name.
        /// </summary>
        /// <returns>UserName. Useful when you've not provided fixed user name, so you can retrive generated user name's value.</returns>
        public static async Task<string> RegisterNewUser(this ToDoLineTestEnv testEnv, string userName = null, string password = "P@ssw0rd")
        {
            userName = userName ?? Guid.NewGuid().ToString("N");

            IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine", odataClientSettings:new ODataClientSettings
            {
                MetadataDocument = ToDoLineMetadata.MetadataString
            });

            await client.UserRegistration()
                .Register(new UserRegistrationDto { UserName = userName, Password = password })
                .ExecuteAsync();

            return userName;
        }

        /// <summary>
        /// Login with provided user name. If registerNewUserByRandomUserName gets provided 'true' then it calls <see cref="RegisterNewUser(ToDoLineTestEnv, string)"/> first.
        /// You've to either provide a valid userName or pass registerNewUserByRandomUserName 'true'. It generates a random userName and returns that to you in case no userName is provided for registration.
        /// </summary>
        public static async Task<ToDoLineClient> LoginInToApp(this ToDoLineTestEnv testEnv, bool registerNewUserByRandomUserName, string userName = null, string password = "P@ssw0rd")
        {
            if (registerNewUserByRandomUserName == true)
            {
                userName = await RegisterNewUser(testEnv, userName);
            }
            else
            {
                if (userName == null)
                    throw new ArgumentNullException(nameof(userName));
            }

            Token token = await testEnv.Server.LoginWithCredentials(userName, password: password, clientId: "ToDoLine", secret: "secret");

            IODataClient odataClient = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine", token: token, odataClientSettings: new ODataClientSettings
            {
                MetadataDocument = ToDoLineMetadata.MetadataString
            });

            return new ToDoLineClient { UserName = userName, ODataClient = odataClient, Token = token };
        }

        public static async Task<UserDto> GetUserByUserName(this IODataClient client, string userName)
        {
            return (await client.Users()
                    .GetAllUsers()
                    .Filter(u => u.UserName.ToLower().Contains(userName.ToLower()))
                    .FindEntriesAsync()).Single();
        }
    }
}
