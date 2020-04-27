using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IGroup_UserRepository : IRepository<Group_User>
    {
        IEnumerable<Group_User> FindAllByUserId(Guid userId);
        Group_User FindOneByGroupIdAndUserId(Guid groupId, Guid userId);
        bool IsExist(Guid groupId, Guid userId);
    }
}
