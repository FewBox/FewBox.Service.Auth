using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IGroupRepository : IBaseRepository<Group, Guid>
    {
        IEnumerable<Group> FindAllByRoot();
        int CountByRoleCode(string roleCode);
    }
}
