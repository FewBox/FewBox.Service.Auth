using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public abstract class CommonRepository<T> : Repository<T> where T : BaseEntity<Guid>
    {
        protected CommonRepository(string tableName, IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base(tableName, ormSession, currentUser)
        {
        }

        protected IList<Guid> GetPrincipalIds(Guid userId)
        {
            List<Guid> principalIds = new List<Guid>();
            var groupIds = this.UnitOfWork.Connection.Query<Group_User>($"select * from group_user where UserId=@UserId", new { UserId = userId }).Select(group_user => group_user.GroupId);
            foreach (Guid groupId in groupIds)
            {
                this.TraversePath(groupId, principalIds);
            }
            var userPrincipalId = this.UnitOfWork.Connection.Query<User>($"select * from `user` where id=@UserId", new { UserId = userId }).Select(user => user.PrincipalId).SingleOrDefault();
            principalIds.Add(userPrincipalId);
            return principalIds;
        }

        private void TraversePath(Guid groupid, List<Guid> ids)
        {
            var group = this.UnitOfWork.Connection.Query<Group>($"select * from `group` where Id=@Id", new { Id = groupid }).SingleOrDefault();
            ids.Add(group.PrincipalId);
            if (group != null && group.ParentId != Guid.Empty)
            {
                this.TraversePath(group.ParentId, ids);
            }
        }
    }
}
