using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class InitProductAdminDto
    {
        public IList<string> SwaggerUrls { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}