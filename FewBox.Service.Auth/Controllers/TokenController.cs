using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using FewBox.Core.Utility.Compress;
using FewBox.Core.Utility.Formatter;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Service.Auth.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class TokenController : ControllerBase
    {
        private ITokenService TokenService { get; set; }
        public TokenController(ITokenService tokenService)
        {
            this.TokenService = tokenService;
        }

        [HttpGet("GetTestJWTToken")]
        [AllowAnonymous]
        public PayloadResponseDto<string> GetTestJWTToken([FromQuery(Name = "apis")] IList<string> apis, string tenant = "fewbox", string jwtKey = "Rushing Smart & Simple ❤", string jwtIssuer = "https://fewbox.com", string audience = "https://fewbox.com", TimeSpan? timeSpan = null)
        {
            if (timeSpan == null)
            {
                timeSpan = TimeSpan.FromHours(2);
            }
            string service = Assembly.GetEntryAssembly().GetName().Name;
            var userProfile = new UserProfile
            {
                Tenant = tenant,
                Id = Guid.NewGuid().ToString(),
                Key = jwtKey,
                Issuer = jwtIssuer,
                Audience = audience,
                GzipApis = GzipUtility.Zip(JsonUtility.Serialize<IList<string>>(apis.Select(api => $"{service}/{api}").ToList())),
                Roles = new List<string> { "Admin" }
            };
            string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.Add(timeSpan.Value));
            return new PayloadResponseDto<string>
            {
                Payload = $"Bearer {token}"
            };
        }

        [HttpGet("Validate")]
        public PayloadResponseDto<string> Validate()
        {
            return new PayloadResponseDto<string>
            {
                Payload = $"Hello World!"
            };
        }
    }
}
