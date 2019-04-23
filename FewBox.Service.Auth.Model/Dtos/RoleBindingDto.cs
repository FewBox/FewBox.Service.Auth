using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class RoleBindingDto : EntityDto<Guid>
    {
        public Guid SecurityObjectId { get; set; }
        public Guid RoleId { get; set; }
    }
}
