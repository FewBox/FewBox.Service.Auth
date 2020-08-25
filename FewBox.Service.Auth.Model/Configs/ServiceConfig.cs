using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ServiceConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<RoleConfig> Roles { get; set; }
        public IList<UserConfig> Users { get; set; }
        public IList<GroupConfig> Groups { get; set; }
        public IList<RoleAssignmentConfig> RoleAssignments { get; set; }
        public IList<ApiConfig> Apis { get; set; }
        public IList<ModuleConfig> Modules { get; set; }
    }
}