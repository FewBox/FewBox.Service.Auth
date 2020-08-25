using System.Collections.Generic;
using FewBox.Service.Auth.Model.Configs;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class BatchInitRequestDto
    {
        public IList<ServiceConfig> Services { get; set; }
    }
}