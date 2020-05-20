using FewBox.Core.Web.Dto;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ModuleDto : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public Guid ParentId { get; set; }
        public Guid SecurityObjectId { get; set; }
        public IList<ModuleDto> Children { get; set; }
    }
}
