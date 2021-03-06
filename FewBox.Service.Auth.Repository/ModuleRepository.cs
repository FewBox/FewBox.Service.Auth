﻿using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class ModuleRepository : CommonRepository<Module>, IModuleRepository
    {
        public ModuleRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("module", ormSession, currentUser)
        {
        }

        public IEnumerable<Module> FindAllByParent(Guid parentId)
        {
            return this.UnitOfWork.Connection.Query<Module>($"select m.*,so.Name,so.Description from {this.TableName} m left join securityobject so on m.SecurityObjectId = so.Id where m.ParentId = @ParentId", new { ParentId = parentId });
        }

        public IEnumerable<Module> FindAllByRoot()
        {
            return this.UnitOfWork.Connection.Query<Module>($"select m.*,so.Name,so.Description from {this.TableName} m left join securityobject so on m.SecurityObjectId = so.Id where m.ParentId = '00000000-0000-0000-0000-000000000000' or m.ParentId is null");
        }

        public Module FindOneByCode(string code)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>($"select m.*,so.Name,so.Description from {this.TableName} m left join securityobject so on m.SecurityObjectId = so.Id where m.Code = @Code", new { Code = code });
        }

        public Module FindOneByName(string name)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>($"select m.*,so.Name,so.Description from {this.TableName} m left join securityobject so on m.SecurityObjectId = so.Id where so.Name = @Name", new { Name = name });
        }

        public new Module FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>($"select m.*,so.Name,so.Description from {this.TableName} m left join securityobject so on m.SecurityObjectId = so.Id where m.Id = @Id", new { Id = id });
        }

        public void ChangeModuleParentId(Guid parentId, IList<Guid> sourceIds)
        {
            this.UnitOfWork.Connection.Execute($"update {this.TableName} set ParentId=@ParentId where Id in @SourceIds", new { ParentId = parentId, SourceIds = sourceIds.ToArray() });
        }

        protected override string GetSaveSegmentSql()
        {
            return "`Code`,ParentId,SecurityObjectId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "`Code`,ParentId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new System.NotImplementedException();
        }

        public int UpdateParent(Guid id, Guid parentId)
        {
            return this.UnitOfWork.Connection.Execute($"update {this.TableName} set ParentId=@ParentId where Id=@Id", new { ParentId = parentId, Id = id });
        }

        public IEnumerable<Module> FindAllByUserId(Guid userId)
        {
            var principals = this.GetPrincipalIds(userId);
            return this.UnitOfWork.Connection.Query<Module, SecurityObject, Module>(
                $@"select * from {this.TableName} left join securityobject on {this.TableName}.SecurityObjectId = securityobject.Id where SecurityObjectId in
                (select SecurityObjectId from role_security where RoleId in
                (select RoleId from principal_role where PrincipalId in @Principals))",
                (module, secuirtyObject) => { module.ServiceId = secuirtyObject.ServiceId; return module; },
                new { Principals = principals });
        }

        public bool IsExist(Guid serviceId, string code)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Code=@Code and SecurityObjectId in (select Id from securityobject where ServiceId = @ServiceId)",
                new { Code = code, ServiceId = serviceId }) > 0;
        }

        public Module FindOneByServiceAndCode(Guid serviceId, string code)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>($"select * from {this.TableName} where Code=@Code and SecurityObjectId in (select Id from securityobject where ServiceId = @ServiceId)",
                new { Code = code, ServiceId = serviceId });
        }
    }
}
