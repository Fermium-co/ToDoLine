using Bit.Core;
using Bit.Core.Contracts;
using Bit.Core.Models;
using Bit.Data;
using Bit.Data.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.Model.Implementations;
using Bit.OData.ActionFilters;
using Bit.OData.Contracts;
using Bit.Owin;
using Bit.Owin.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using System.Web.Http;
using ToDoLine.Controller;
using ToDoLine.Data;
using ToDoLine.Security;

[assembly: ODataModule("ToDoLine")]

namespace ToDoLine
{
    public class Startup : AutofacAspNetCoreAppStartup
    {
        public Startup(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            AspNetCoreAppEnvironmentsProvider.Current.Init();
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultAppModulesProvider.Current = new ToDoLineAppModulesProvider();

            return base.ConfigureServices(services);
        }
    }

    public class ToDoLineAppModulesProvider : IAppModule, IAppModulesProvider
    {
        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            AssemblyContainer.Current.Init();

            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo());

            dependencyManager.RegisterDefaultAspNetCoreApp();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            }).Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            }).Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseResponseCompression();
                aspNetCoreApp.UseStaticFiles();
            });

            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            dependencyManager.RegisterMetadata();

            dependencyManager.RegisterDefaultWebApiAndODataConfiguration();

            dependencyManager.RegisterWebApiMiddleware(webApiDependencyManager =>
            {
                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();

                webApiDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());

                    httpConfiguration.EnableMultiVersionWebApiSwaggerWithUI();
                });
            });

            dependencyManager.RegisterODataMiddleware(odataDependencyManager =>
            {
                odataDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new DefaultODataAuthorizeAttribute());

                    httpConfiguration.EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", $"ToDoLine-Api");
                        c.ApplyDefaultODataConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                odataDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
            });

            dependencyManager.Register<IDbConnectionProvider, DefaultDbConnectionProvider<SqlConnection>>();
            dependencyManager.RegisterEfCoreDbContext<ToDoLineDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(serviceProvider.GetRequiredService<IDbConnectionProvider>().GetDbConnection(serviceProvider.GetService<IConfiguration>().GetConnectionString("AppConnectionString"), rollbackOnScopeStatusFailure: true));
            });
            dependencyManager.RegisterRepository(typeof(EfCoreRepository<>).GetTypeInfo());

            dependencyManager.RegisterDtoEntityMapper();
            dependencyManager.RegisterMapperConfiguration<DefaultMapperConfiguration>();
            dependencyManager.RegisterMapperConfiguration<ToDoLineMapperConfiguration>();

            dependencyManager.RegisterSingleSignOnServer<ToDoLineUserService, ToDoLineClientsProvider>();

            dependencyManager.RegisterIndexPageMiddlewareUsingDefaultConfiguration();
        }
    }
}
