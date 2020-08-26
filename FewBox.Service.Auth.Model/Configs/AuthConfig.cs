using System;

namespace FewBox.Service.Auth.Model.Configs
{
    public class AuthConfig
    {
        public TimeSpan ExpireTime { get; set; }
        public OrmConfigurationTypeConfig OrmConfigurationType { get; set; }
        public string Type {
            set
            {
                this.OrmConfigurationType = (OrmConfigurationTypeConfig)Enum.Parse(typeof(OrmConfigurationTypeConfig), value);
            }
        }
    }
}