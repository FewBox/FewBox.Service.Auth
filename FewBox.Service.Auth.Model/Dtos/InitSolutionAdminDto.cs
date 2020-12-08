using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class InitSolutionAdminDto
    {
        public IList<string> SwaggerUrls { get; set; }
        public string Email { get; set; }
    }
}