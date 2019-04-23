using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Module : SecurityObject
    {
        public string Key { get; set; }
        public Guid ParentId { get; set; }
        public Guid SecurityObjectId { get; set; }
    }
}
