using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IGroup_UserRepository : IBaseRepository<Group_User, Guid>
    {
        IEnumerable<Group_User> FindAllByUserId(Guid userId);
    }
}
