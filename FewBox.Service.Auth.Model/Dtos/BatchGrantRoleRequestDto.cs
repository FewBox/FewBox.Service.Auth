using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class BatchGrantRoleRequestDto
    {
        public IList<Guid> Ids { get; set; }
        public IList<Guid> RoleIds { get; set; }
    }
}
