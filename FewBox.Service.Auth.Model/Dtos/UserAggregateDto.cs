using FewBox.Core.Web.Dto;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class UserAggregateDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public IList<GroupDto> Groups { get; set; }
        public IList<RoleDto> Roles { get; set; }
    }
}
