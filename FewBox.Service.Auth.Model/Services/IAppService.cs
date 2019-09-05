using FewBox.Service.Auth.Model.Dtos;

namespace FewBox.Service.Auth.Model.Services
{
    public interface IAppService
    {
        HealthyDto GetHealtyInfo();
    }
}