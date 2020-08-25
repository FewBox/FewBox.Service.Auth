using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ActionConfig
    {
        public IList<string> Roles { get; set; }
        public string Name { get; set; }
    }
}
