using Dapper;
using System;
using System.Collections.Generic;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Repository
{
    public class Group_UserRepository : BaseRepository<Group_User, Guid>, IGroup_UserRepository
    {
        public Group_UserRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("group_user", ormSession, currentUser)
        {
        }

        public IEnumerable<Group_User> FindAllByUserId(Guid userId)
        {
            return this.UnitOfWork.Connection.Query<Group_User>($"select * from {this.TableName} where UserId=@UserId", new { UserId = userId });
        }

        public Group_User FindOneByGroupIdAndUserId(Guid groupId, Guid userId)
        {
            return this.UnitOfWork.Connection.QueryFirstOrDefault<Group_User>($"select * from {this.TableName} where GroupId=@GroupId and UserId=@UserId", new { UserId = userId, GroupId = groupId });
        }

        public bool IsExist(Guid groupId, Guid userId)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(Id) from {this.TableName} where GroupId=@Groupid and UserId=@UserId", new { UserId = userId, GroupId = groupId }) > 0;
        }

        protected override string GetSaveSegmentSql()
        {
            return "GroupId,UserId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "GroupId,UserId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new System.NotImplementedException();
        }
    }
}
