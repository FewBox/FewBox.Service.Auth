using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class InitProductRoleDto
    {
        public string ProductName { get; set; }
        public IList<InitProductRoleApiDto> FreeRoleApis { get; set; }
        public IList<InitProductRoleApiDto> ProRoleApis { get; set; }
    }
}