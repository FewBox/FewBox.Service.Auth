using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IModuleRepository : IBaseRepository<Module, Guid>
    {
        IEnumerable<Module> FindAllByRoot();
        IEnumerable<Module> FindAllByParent(Guid parentId);
        Module FindOneByKey(string key);
        void ChangeModuleParentId(Guid parentId, IList<Guid> sourceIds);
    }
}
