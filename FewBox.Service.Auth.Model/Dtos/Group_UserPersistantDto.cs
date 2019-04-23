using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class Group_UserPersistantDto
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
