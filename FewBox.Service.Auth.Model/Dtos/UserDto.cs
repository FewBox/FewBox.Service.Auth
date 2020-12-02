using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class UserDto : EntityDto
    {
        public UserTypeDto Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Guid PrincipalId { get; set; }
        public Guid TenantId { get; set; }
    }
}
