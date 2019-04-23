using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class Principal_RoleDto : EntityDto<Guid>
    {
        public Guid PrincipalId { get; set; }
        public Guid RoleId { get; set; }
    }
}
