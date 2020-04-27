using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public bool IsExist(string name)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Name = @Name",
                new { Name = name }) > 0;
        }

        public bool IsExist(string name, string code)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Name = @Name and Code = @Code",
                new { Name = name, Code = code }) > 0;
        }

        public Role FindOneByName(string name)
        {
           return this.UnitOfWork.Connection.QuerySingleOrDefault<Role>($"select * from {this.TableName} where Name = @Name",
                new { Name = name });
        }

        public Role FindOneByNameAndCode(string name, string code)
        {
           return this.UnitOfWork.Connection.QuerySingleOrDefault<Role>($"select * from {this.TableName} where Name = @Name and Code = @Code",
                new { Name = name, Code = code });
        }

        public RoleRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("role", ormSession, currentUser)
        {
        }

        public IEnumerable<Role> FindAllByApiId(Guid apiId)
        {
            return this.UnitOfWork.Connection.Query<Role>($"select * from {this.TableName} where Id in (select RoleId from role_security where SecurityObjectId in (select SecurityObjectId from api where Id=@ApiId))", new { ApiId = apiId });
        }

        public IEnumerable<Role> FindAllByModuleId(Guid moduleId)
        {
            return this.UnitOfWork.Connection.Query<Role>($"select * from {this.TableName} where Id in (select RoleId from role_security where SecurityObjectId in (select SecurityObjectId from module where Id=@ModuleId))", new { ModuleId = moduleId });
        }

        public IEnumerable<Role> FindAllByGroupId(Guid groupId)
        {
            return this.UnitOfWork.Connection.Query<Role>($"select * from {this.TableName} where Id in (select RoleId from principal_role where PrincipalId in (select PrincipalId from `group` where Id=@GroupId))", new { GroupId = groupId });
        }

        public IEnumerable<Role> FindAllByUserId(Guid userId)
        {
            return this.UnitOfWork.Connection.Query<Role>($"select * from {this.TableName} where Id in (select RoleId from principal_role where PrincipalId in (select PrincipalId from user where Id=@UserId))", new { UserId = userId });
        }

        public Role FindOneByCode(string code)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Role>($"select * from {this.TableName} where Code=@Code", new { Code = code });
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
