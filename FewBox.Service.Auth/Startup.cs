using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Filter;
using FewBox.Core.Web.Orm;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using FewBox.Service.Auth.Domain;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Service.Auth.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;
using FewBox.Service.Auth.Model.Configs;
using Microsoft.AspNetCore.Routing;

namespace FewBox.Service.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options=>{
                options.Filters.Add<ExceptionAsyncFilter>();
                options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<RouteOptions>(options=>{
                options.LowercaseUrls=true;
            });
            services.AddCors();
            services.AddAutoMapper();
            services.AddMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            var apiConfig = this.Configuration.GetSection("ApiConfig").Get<ApiConfig>();
            services.AddSingleton(apiConfig);
            services.AddScoped<ITokenService, JWTToken>();
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            services.AddScoped<IAuthenticationService, LocalAuthenticationService>();
            services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            services.AddScoped<IOrmSession, MySqlSession>();
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            services.AddScoped<IAppRepository, AppRepository>();
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
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILDAPService, LDAPService>();
            services.AddScoped<IExceptionHandler, ConsoleExceptionHandler>();
            services.AddScoped<ITraceLogger, ConsonleTraceLogger>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>(); 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
            services.AddOpenApiDocument(config => {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "FewBox Auth API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "https://fewbox.com/terms";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "XL Pang",
                        Email = "support@fewbox.com",
                        Url = "https://fewbox.com/support"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under license",
                        Url = "https://raw.githubusercontent.com/FewBox/FewBox.Service.Auth/master/LICENSE"
                    };
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT", new List<string>{"API"}, new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Bearer [Token]",
                        In = SwaggerSecurityApiKeyLocation.Header
                    })
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
            app.UseStaticFiles();
            app.UseSwagger();
            if (env.IsDevelopment() || env.IsStaging())  
            {
                app.UseSwagger();  
                app.UseSwaggerUi3();  
            }
            else
            {
                app.UseReDoc();
            }
            app.UseCors();
        }
    }
}
