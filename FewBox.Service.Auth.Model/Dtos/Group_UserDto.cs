using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class Group_UserDto : EntityDto<Guid>
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
