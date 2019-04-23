using FewBox.Service.Auth.Model.Dtos;

namespace FewBox.Service.Auth.Model.Services
{
    public interface IModuleService
    {
        ModuleDto GetTree(string key);
    }
}
