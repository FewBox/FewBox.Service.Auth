using FewBox.Service.Auth.Model.Configs;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Services;

namespace FewBox.Service.Auth.Domain
{
    public class AppService : IAppService
    {
        private HealthyConfig HealthyConfig { get; set; }
        public AppService(HealthyConfig healthyConfig)
        {
            this.HealthyConfig = healthyConfig;
        }

        public HealthyDto GetHealtyInfo()
        {
            return new HealthyDto{
                Version = this.HealthyConfig.Version
            };
        }
    }
}