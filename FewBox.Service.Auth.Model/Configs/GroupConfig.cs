using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class GroupConfig
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public IList<UserConfig> Users { get; set; }
    }
}