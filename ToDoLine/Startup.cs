﻿using Bit.Core.Contracts;
using Bit.Data;
using Bit.Data.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.Model.Implementations;
using Bit.OData.ActionFilters;
using Bit.OData.Contracts;
using Bit.Owin;
using Bit.Owin.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Application;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Validation;
using ToDoLine;
using ToDoLine.Controller;
using ToDoLine.Data;
using ToDoLine.Security;

[assembly: ODataModule("ToDoLine")]
[assembly: AppModule(typeof(ToDoLineAppModule))]

namespace ToDoLine.Dto
{
    public partial class ToDoItemDto : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = DefaultDependencyManager.Current.Resolve<IHttpContextAccessor>().HttpContext;

            validationContext.InitializeServiceProvider(type => context.RequestServices.GetRequiredService(type));

            var dateTimeProvider = validationContext.GetRequiredService<IDateTimeProvider>();
            var requestInformationProvider = validationContext.GetRequiredService<IRequestInformationProvider>();

            yield break;
        }
    }
}

namespace ToDoLine
{
    public class Startup : AspNetCoreAppStartup
    {

    }

    public class ToDoLineAppModule : IAppModule
    {
        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo());

            dependencyManager.RegisterDefaultAspNetCoreApp();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
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
                    IEnumerable<ModelValidatorProvider> modelValidatorProviders = httpConfiguration.Services.GetModelValidatorProviders();

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
