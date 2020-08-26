using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class InitialConfig
    {
        public string Tenant { get; set; }
        public string SystemEmail { get; set; }
        public IList<ServiceConfig> Services { get; set; }
    }
}
