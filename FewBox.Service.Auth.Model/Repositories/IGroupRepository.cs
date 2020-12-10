using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Group FindOneByName(string name);
        IEnumerable<Group> FindAllByRoot();
        int CountByRoleCode(string roleCode);
        int UpdateParent(Guid id, Guid parentId);
    }
}
