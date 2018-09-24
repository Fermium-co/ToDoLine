using Bit.Test;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToDoLine.Test
{
    public class ToDoLineTestEnv : TestEnvironmentBase
    {
        public ToDoLineTestEnv(TestEnvironmentArgs args = null)
            : base(ApplyArgsDefaults(args))
        {

        }

        private static TestEnvironmentArgs ApplyArgsDefaults(TestEnvironmentArgs args)
        {
            args = args ?? new TestEnvironmentArgs();
            args.CustomAppModulesProvider = args.CustomAppModulesProvider ?? new ToDoLineAppModulesProvider();
            args.UseAspNetCore = true;
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
