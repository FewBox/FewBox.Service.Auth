using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class GroupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
