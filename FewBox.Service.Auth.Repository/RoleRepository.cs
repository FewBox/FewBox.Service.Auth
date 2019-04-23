using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class RoleRepository : BaseRepository<Role, Guid>, IRoleRepository
    {
        public RoleRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("role", ormSession, currentUser)
        {
        }

        public IEnumerable<Role> FindAllByApiId(Guid apiId)
        {
            return this.UnitOfWork.Connection.Query<Role>(String.Format(@"select * from {0} where Id in (select RoleId from role_security where SecurityObjectId in (select SecurityObjectId from api where Id=@ApiId))", this.TableName), new { ApiId = apiId });
        }

        public IEnumerable<Role> FindAllByModuleId(Guid moduleId)
        {
            return this.UnitOfWork.Connection.Query<Role>(String.Format(@"select * from {0} where Id in (select RoleId from role_security where SecurityObjectId in (select SecurityObjectId from module where Id=@ModuleId))", this.TableName), new { ModuleId = moduleId });
        }

        public IEnumerable<Role> FindAllByGroupId(Guid groupId)
        {
            return this.UnitOfWork.Connection.Query<Role>(String.Format(@"select * from {0} where Id in (select RoleId from principal_role where PrincipalId in (select PrincipalId from `group` where Id=@GroupId))", this.TableName), new { GroupId = groupId });
        }

        public IEnumerable<Role> FindAllByPermissionId(Guid permissionId)
        {
            return this.UnitOfWork.Connection.Query<Role>(String.Format(@"select * from {0} where Id in (select RoleId from role_permission where PermissionId in (select PermissionId from permission where Id=@PermissionId))", this.TableName), new { PermissionId = permissionId });
        }

        public IEnumerable<Role> FindAllByUserId(Guid userId)
        {
            return this.UnitOfWork.Connection.Query<Role>(String.Format(@"select * from {0} where Id in (select RoleId from principal_role where PrincipalId in (select PrincipalId from user where Id=@UserId))", this.TableName), new { UserId = userId });
        }

        public Role FindOneByCode(string code)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Role>(String.Format(@"select * from {0} where Code=@Code", this.TableName), new { Code = code });
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name,Code,Description";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name,Code,Description";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
