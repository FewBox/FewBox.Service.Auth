using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ApiDto : EntityDto
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Guid SecurityObjectId { get; set; }
    }
}
