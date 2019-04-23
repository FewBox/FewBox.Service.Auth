using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Group : Principal
    {
        public Guid ParentId { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
