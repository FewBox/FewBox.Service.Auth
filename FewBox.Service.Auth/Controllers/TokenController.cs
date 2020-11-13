using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
            var apiClaims = apis.Select(api => new Claim(TokenClaims.Api, $"{service}/{api}"));
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
            claims.AddRange(apiClaims);
            var userInfo = new UserInfo
            {
                Tenant = tenant,
                Id = Guid.NewGuid(),
                Key = jwtKey,
                Issuer = jwtIssuer,
                Audience = audience,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, DateTime.Now.Add(timeSpan.Value));
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
