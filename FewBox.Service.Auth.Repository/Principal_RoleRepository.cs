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
            this.UnitOfWork.Connection.Query<Principal_Role>(String.Format(@"delete from {0} where PrincipalId=@PrincipalId", this.TableName), new { PrincipalId = principalId });
        }

        public IEnumerable<Principal_Role> FindAllByPrincipalId(Guid principalId)
        {
            return this.UnitOfWork.Connection.Query<Principal_Role>(String.Format(@"select * from {0} where PrincipalId=@PrincipalId", this.TableName), new { PrincipalId = principalId });
        }

        public IEnumerable<Principal_Role> FindAllByPrincipalIds(IList<Guid> principalIds)
        {
            return this.UnitOfWork.Connection.Query<Principal_Role>(String.Format(@"select * from {0} where PrincipalId in @PincipalIds", this.TableName), new { PincipalIds = principalIds });
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
