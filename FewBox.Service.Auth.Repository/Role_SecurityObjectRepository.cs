using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class Role_SecurityObjectRepository : BaseRepository<Role_SecurityObject, Guid>, IRole_SecurityObjectRepository
    {
        public Role_SecurityObjectRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("role_security", ormSession, currentUser)
        {
        }

        public void DeleteBySecurityId(Guid securityObjectId)
        {
            this.UnitOfWork.Connection.Query<Role_SecurityObject>(String.Format(@"delete from {0} where SecurityObjectId=@SecurityObjectId", this.TableName), new { SecurityObjectId = securityObjectId });
        }

        public IEnumerable<Role_SecurityObject> FindAllBySecurityId(Guid securityObjectId)
        {
            return this.UnitOfWork.Connection.Query<Role_SecurityObject>(String.Format(@"select * from {0} where SecurityObjectId=@SecurityObjectId", this.TableName), new { SecurityObjectId = securityObjectId });
        }

        protected override string GetSaveSegmentSql()
        {
            return "SecurityObjectId,RoleId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "SecurityObjectId,RoleId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
