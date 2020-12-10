using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ModuleConfig
    {
        public IList<string> DefaultRoles { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentName { get; set; }
    }
}