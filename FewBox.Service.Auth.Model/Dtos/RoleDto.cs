using System;
using FewBox.Core.Web.Dto;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class RoleDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
