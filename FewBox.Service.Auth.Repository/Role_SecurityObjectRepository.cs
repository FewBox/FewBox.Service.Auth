using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class Role_SecurityObjectRepository : Repository<Role_SecurityObject>, IRole_SecurityObjectRepository
    {
        public Role_SecurityObjectRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("role_security", ormSession, currentUser)
        {
        }

        public void DeleteBySecurityId(Guid securityObjectId)
        {
            this.UnitOfWork.Connection.Query<Role_SecurityObject>($"delete from {this.TableName} where SecurityObjectId=@SecurityObjectId", new { SecurityObjectId = securityObjectId });
        }

        public IEnumerable<Role_SecurityObject> FindAllBySecurityId(Guid securityObjectId)
        {
            return this.UnitOfWork.Connection.Query<Role_SecurityObject>($"select * from {this.TableName} where SecurityObjectId=@SecurityObjectId", new { SecurityObjectId = securityObjectId });
        }

        public Role_SecurityObject FindOneByRoleIdAndSecurityObjectId(Guid roleId, Guid securityObjectId)
        {
            return this.UnitOfWork.Connection.QueryFirstOrDefault<Role_SecurityObject>($"select * from {this.TableName} where RoleId=@RoleId and SecurityObjectId=@SecurityObjectId", new { RoleId = roleId, SecurityObjectId = securityObjectId});
        }

        public bool IsExist(Guid roleId, Guid securityObjectId)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(Id) from {this.TableName} where RoleId=@RoleId and SecurityObjectId=@SecurityObjectId", new { RoleId = roleId, SecurityObjectId = securityObjectId}) > 0;
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
