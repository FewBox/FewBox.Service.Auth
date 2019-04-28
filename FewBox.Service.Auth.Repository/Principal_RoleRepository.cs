using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class Principal_RoleRepository : BaseRepository<Principal_Role, Guid>, IPrincipal_RoleRepository
    {
        public Principal_RoleRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("principal_role", ormSession, currentUser)
        {
        }

        public void DeleteByPrincipalId(Guid principalId)
        {
            this.UnitOfWork.Connection.Query<Principal_Role>($"delete from {this.TableName} where PrincipalId=@PrincipalId", new { PrincipalId = principalId });
        }

        public IEnumerable<Principal_Role> FindAllByPrincipalId(Guid principalId)
        {
            return this.UnitOfWork.Connection.Query<Principal_Role>($"select * from {this.TableName} where PrincipalId=@PrincipalId", new { PrincipalId = principalId });
        }

        public IEnumerable<Principal_Role> FindAllByPrincipalIds(IList<Guid> principalIds)
        {
            return this.UnitOfWork.Connection.Query<Principal_Role>($"select * from {this.TableName} where PrincipalId in @PincipalIds", new { PincipalIds = principalIds });
        }

        public Principal_Role FindOneByPrincipalIdAndRoleId(Guid principalId, Guid roleId)
        {
            return this.UnitOfWork.Connection.QueryFirstOrDefault<Principal_Role>($"select * from {this.TableName} where PrincipalId=@PrincipalId and RoleId=@RoleId", new { PrincipalId =  principalId, RoleId = roleId });
        }

        public bool IsExist(Guid principalId, Guid roleId)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(Id) from {this.TableName} where PrincipalId=@PrincipalId and RoleId=@RoleId", new { PrincipalId =  principalId, RoleId = roleId }) > 0;
        }

        protected override string GetSaveSegmentSql()
        {
            return "PrincipalId,RoleId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "PrincipalId,RoleId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
