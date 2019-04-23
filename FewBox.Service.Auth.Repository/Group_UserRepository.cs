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
            return this.UnitOfWork.Connection.Query<Group_User>(String.Format(@"select * from {0} where UserId=@UserId", this.TableName), new { UserId = userId });
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
