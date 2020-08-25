using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ApiConfig
    {
        public IList<string> DefaultRoles { get; set; }
        public string Controller { get; set; }
        public IList<ActionConfig> Actions { get; set; }
    }
}
