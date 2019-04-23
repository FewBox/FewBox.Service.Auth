using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Token;
using FewBox.Service.Auth.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRouting(options => options.LowercaseUrls = true);
            var jwtConfig = this.Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            services.AddSingleton(jwtConfig);
            services.AddScoped<ITokenService, JWTToken>();
            services.AddSingleton<IAuthorizationHandler, RemoteRoleHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, RemoteRoleAuthorizationPolicyProvider>();
            services.AddSingleton<IRemoteAuthenticationService, LocalAuthenticationService>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>(); 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
