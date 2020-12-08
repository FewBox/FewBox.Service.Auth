using FewBox.Service.Auth.Domain;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Service.Auth.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FewBox.Core.Web.Extension;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using FewBox.Service.Auth.Model.Configs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FewBox.SDK.Extension;
using FewBox.SDK.Auth;
using System.Reflection;

namespace FewBox.Service.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFewBox(FewBoxDBType.MySQL, FewBoxAuthType.Payload, new ApiVersion(1, 0, "alpha1"));
            services.AddFewBoxSDK(FewBoxIntegrationType.MessageQueue, FewBoxListenerHostType.Web, FewBoxListenerType.Plan);
            // Config
            var authConfig = this.Configuration.GetSection("AuthConfig").Get<AuthConfig>();
            services.AddSingleton(authConfig);
            var initialConfig = this.Configuration.GetSection("InitialConfig").Get<InitialConfig>();
            services.AddSingleton(initialConfig);
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v1", new[] { "1-alpha1", "1-beta1", "1" }, "v1");
            });
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v2", new[] { "2-alpha2", "2-beta2", "2" }, "v2");
            });
            // Used for Application.
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IPrincipalRepository, PrincipalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISecurityObjectRepository, SecurityObjectRepository>();
            services.AddScoped<IApiRepository, ApiRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IPrincipal_RoleRepository, Principal_RoleRepository>();
            services.AddScoped<IRole_SecurityObjectRepository, Role_SecurityObjectRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IGroup_UserRepository, Group_UserRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILDAPService, LDAPService>();
            services.AddScoped<IMQPlanHandler, MQPlanHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFewBox(new List<string> { "/swagger/v1/swagger.json", "/swagger/v2/swagger.json" });
        }

        private void InitAspNetCoreOpenApiDocumentGeneratorSettings(AspNetCoreOpenApiDocumentGeneratorSettings config, string documentName, string[] apiGroupNames, string documentVersion)
        {
            config.DocumentName = documentName;
            config.ApiGroupNames = apiGroupNames;
            config.PostProcess = document =>
            {
                this.InitDocumentInfo(document, documentVersion);
            };
            config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
            config.DocumentProcessors.Add(
                new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Bearer [Token]",
                    In = OpenApiSecurityApiKeyLocation.Header
                })
            );
        }

        private void InitDocumentInfo(OpenApiDocument document, string version)
        {
            document.Info.Version = version;
            document.Info.Title = "FewBox Auth Api";
            document.Info.Description = "FewBox Auth, for more information please visit the 'https://fewbox.com'";
            document.Info.TermsOfService = "https://fewbox.com/terms";
            document.Info.Contact = new OpenApiContact
            {
                Name = "FewBox",
                Email = "support@fewbox.com",
                Url = "https://fewbox.com/support"
            };
            document.Info.License = new OpenApiLicense
            {
                Name = "Use under license",
                Url = "https://fewbox.com/license"
            };
            string service = Assembly.GetEntryAssembly().GetName().Name;
            document.Info.ExtensionData = new Dictionary<string, object> {
                {"Service", service}
            };
        }
    }
}
