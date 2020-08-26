using System;

namespace FewBox.Service.Auth.Model.Configs
{
    public class AuthConfig
    {
        public TimeSpan ExpireTime { get; set; }
        public OrmConfigurationTypeConfig OrmConfigurationType
        {
            get
            {
                if (String.IsNullOrEmpty(this.Type))
                {
                    return OrmConfigurationTypeConfig.Unknown;
                }
                else
                {
                    return (OrmConfigurationTypeConfig)Enum.Parse(typeof(OrmConfigurationTypeConfig), this.Type);
                }
            }
        }
        public string Type { get; set; }
    }
}