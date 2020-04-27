using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IModuleRepository : IRepository<Module>
    {
        bool IsExist(Guid serviceId, string code);
        Module FindOneByServiceAndCode(Guid serviceId, string code);
        IEnumerable<Module> FindAllByRoot();
        IEnumerable<Module> FindAllByParent(Guid parentId);
        IEnumerable<Module> FindAllByUserId(Guid userId);
        Module FindOneByKey(string key);
        void ChangeModuleParentId(Guid parentId, IList<Guid> sourceIds);
        int UpdateParent(Guid id, Guid parentId);
    }
}
