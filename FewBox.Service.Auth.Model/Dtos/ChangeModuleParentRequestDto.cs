using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ChangeModuleParentRequestDto
    {
        public Guid ParentId { get; set; }
        public IList<Guid> SourceIds { get; set; }
    }
}
