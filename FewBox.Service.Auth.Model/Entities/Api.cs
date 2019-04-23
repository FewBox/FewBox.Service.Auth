using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Api : SecurityObject
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Guid SecurityObjectId { get; set; }
    }
}
