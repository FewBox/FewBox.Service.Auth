﻿using AutoMapper;
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
using FewBox.Core.Web.Error;
using FewBox.Core.Utility.Net;
using FewBox.Core.Utility.Formatter;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json;
using FewBox.Core.Web.Log;
using FewBox.Core.Web.Notification;

namespace FewBox.Service.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RestfulUtility.IsCertificateNeedValidate = false;
            RestfulUtility.IsLogging = true; // Todo: Need to remove.
            JsonUtility.IsCamelCase = true;
            JsonUtility.IsNullIgnore = true;
            HttpUtility.IsCertificateNeedValidate = false;
            HttpUtility.IsEnsureSuccessStatusCode = false;
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options=>{
                if (this.HostingEnvironment.EnvironmentName != "Test")
                {
                    options.Filters.Add<ExceptionAsyncFilter>();
                }
                if (this.HostingEnvironment.EnvironmentName == "Development")
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                }
                options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            })
            .AddJsonOptions(options=>{
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins("https://fewbox.com").AllowAnyMethod().AllowAnyHeader();
                        });
                    options.AddPolicy("all",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                        });

                });
            services.AddAutoMapper();
            services.AddMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<IExceptionProcessorService, ExceptionProcessorService>();
            // Used for Config.
            // Used for [Authorize(Policy="JWTRole_ControllerAction")].
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            var securityConfig = this.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>();
            services.AddSingleton(securityConfig);
            var healthyConfig = this.Configuration.GetSection("HealthyConfig").Get<HealthyConfig>();
            services.AddSingleton(healthyConfig);
            var logConfig = this.Configuration.GetSection("LogConfig").Get<LogConfig>();
            services.AddSingleton(logConfig);
            var notificationConfig = this.Configuration.GetSection("NotificationConfig").Get<NotificationConfig>();
            services.AddSingleton(notificationConfig);
            var authConfig = this.Configuration.GetSection("AuthConfig").Get<AuthConfig>();
            services.AddSingleton(authConfig);
            var apiConfig = this.Configuration.GetSection("ApiConfig").Get<ApiConfig>();
            services.AddSingleton(apiConfig);
            var externalApiConfig = this.Configuration.GetSection("ExternalApiConfig").Get<ExternalApiConfig>();
            if (externalApiConfig != null)
            {
                services.AddSingleton(externalApiConfig);
            }
            else
            {
                services.AddSingleton(new ExternalApiConfig { });
            }
            // Used for RBAC AOP.
            services.AddScoped<IAuthorizationHandler, RoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();
            services.AddScoped<IAuthService, LocalAuthService>();
            // Used for ORM.
            services.AddScoped<IOrmConfiguration, AppSettingOrmConfiguration>();
            services.AddScoped<IOrmSession, MySqlSession>(); // Note: MySql
            // services.AddScoped<IOrmSession, SQLiteSession>(); // Note: SQLite
            services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
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
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILDAPService, LDAPService>();
            // Used for Exception&Log AOP.
            // services.AddScoped<ILogHandler, ConsoleLogHandler>();
            // services.AddScoped<INotificationHandler, ConsoleNotificationHandler>();
            services.AddScoped<ILogHandler, ServiceLogHandler>();
            services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
            services.AddScoped<ITryCatchService, TryCatchService>();
            // Used for IHttpContextAccessor&IActionContextAccessor context.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Used for JWT.
            services.AddScoped<ITokenService, JWTToken>();
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
            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "FewBox Auth API";
                    document.Info.Description = "FewBox Auth, for more information please visit the 'https://fewbox.com'";
                    document.Info.TermsOfService = "https://fewbox.com/terms";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "FewBox",
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
                    new SecurityDefinitionAppender("JWT", new List<string> { "API" }, new SwaggerSecurityScheme
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

            //app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseCors("all");
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseCors("all");
                app.UseReDoc();
            }
            app.UseMvc();
        }
    }
}
