using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class AppRegistryRequestDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string TenantAdminName { get; set; }
        public string TenantAdminPassword { get; set; }
        public string ValidateCode { get; set; }
    }
}