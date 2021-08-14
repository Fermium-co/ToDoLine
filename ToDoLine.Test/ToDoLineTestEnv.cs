using Bit.Core;
using Bit.Owin.Implementations;
using Bit.Test;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ToDoLine.Util;

namespace ToDoLine.Test
{
    public class ToDoLineTestEnv : TestEnvironmentBase
    {
        static ToDoLineTestEnv()
        {
            if (!Environment.Is64BitProcess)
                throw new InvalidOperationException("Please run tests in x64 process");

            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../ToDoLine");
            AspNetCoreAppEnvironmentsProvider.Current.Configuration = ToDoLineConfigurationProvider.GetConfiguration();
            IHostEnvironment hostEnv = A.Fake<IHostEnvironment>();
            hostEnv.EnvironmentName = Environments.Development;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);
            hostEnv.ApplicationName = "Redemption";
            AspNetCoreAppEnvironmentsProvider.Current.HostingEnvironment = hostEnv;
            AspNetCoreAppEnvironmentsProvider.Current.Init();
            AspNetCoreAppEnvironmentsProvider.Current.Use();
        }

        public ToDoLineTestEnv(TestEnvironmentArgs args = null)
            : base(ApplyArgsDefaults(args))
        {

        }

        private static TestEnvironmentArgs ApplyArgsDefaults(TestEnvironmentArgs args)
        {
            args = args ?? new TestEnvironmentArgs();
            args.CustomAppModulesProvider = args.CustomAppModulesProvider ?? new ToDoLineAppModulesProvider();
            return args;
        }

        protected override List<Func<TypeInfo, bool>> GetAutoProxyCreationIncludeRules()
        {
            List<Func<TypeInfo, bool>> baseList = base.GetAutoProxyCreationIncludeRules();
            baseList.Add(implementationType => implementationType.Assembly == typeof(Startup).GetTypeInfo().Assembly);
            return baseList;
        }
    }
}
