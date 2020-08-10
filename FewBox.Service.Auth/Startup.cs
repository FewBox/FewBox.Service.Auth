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
using FewBox.Service.Auth.Model.Configs;
using FewBox.Core.Web.Error;
using FewBox.Core.Utility.Net;
using FewBox.Core.Utility.Formatter;
using Microsoft.AspNetCore.Mvc.Authorization;
using FewBox.Core.Web.Notification;
using NSwag;
using NSwag.Generation.Processors.Security;
using Sentry.Extensibility;
using Microsoft.Extensions.Hosting;
using FewBox.Core.Web.Sentry;

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
            RestfulUtility.IsCertificateNeedValidate = false;
            RestfulUtility.IsLogging = true; // Todo: Need to remove.
            JsonUtility.IsCamelCase = true;
            JsonUtility.IsNullIgnore = true;
            HttpUtility.IsCertificateNeedValidate = false;
            HttpUtility.IsEnsureSuccessStatusCode = false;
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options =>
            {
                if (this.Environment.IsDevelopment())
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                }
                options.Filters.Add<TransactionAsyncFilter>();
                options.Filters.Add<TraceAsyncFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins("https://fewbox.com", "https://figma.com")
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });
                    options.AddPolicy("dev",
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            //.AllowAnyOrigin()
                            .WithOrigins("http://localhost", "https://localhost")
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                        });

                });
            services.AddAutoMapper(typeof(Startup));
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
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILDAPService, LDAPService>();
            // Used for Exception.
            // services.AddScoped<INotificationHandler, ConsoleNotificationHandler>();
            services.AddScoped<INotificationHandler, ServiceNotificationHandler>();
            services.AddScoped<ITryCatchService, TryCatchService>();
            // Used for IHttpContextAccessor&IActionContextAccessor context.
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Used for JWT.
            services.AddScoped<ITokenService, JWTTokenService>();
            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection = this.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                }
            )
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
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
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "FewBox",
                        Email = "support@fewbox.com",
                        Url = "https://fewbox.com/support"
                    };
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "Use under license",
                        Url = "https://raw.githubusercontent.com/FewBox/FewBox.Service.Auth/master/LICENSE"
                    };
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT", new List<string> { "API" }, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Bearer [Token]",
                        In = OpenApiSecurityApiKeyLocation.Header
                    })
                );
            });
            // Used for Sentry
            services.AddTransient<ISentryEventProcessor, SentryEventProcessor>();
            services.AddSingleton<ISentryEventExceptionProcessor, SentryEventExceptionProcessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseCors("dev");
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsStaging())
            {
                app.UseCors();
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsProduction())
            {
                app.UseCors();
                app.UseReDoc();
                app.UseHsts();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
