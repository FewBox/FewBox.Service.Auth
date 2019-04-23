using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class ModuleRepository : BaseRepository<Module, Guid>, IModuleRepository
    {
        public ModuleRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("module", ormSession, currentUser)
        {
        }

        public IEnumerable<Module> FindAllByParent(Guid parentId)
        {
            return this.UnitOfWork.Connection.Query<Module>(String.Format(@"select m.*,so.Name,so.Description from {0} m left join securityobject so on m.SecurityObjectId = so.Id where m.ParentId = @ParentId", this.TableName), new { ParentId = parentId });
        }

        public IEnumerable<Module> FindAllByRoot()
        {
            return this.UnitOfWork.Connection.Query<Module>(String.Format(@"select m.*,so.Name,so.Description from {0} m left join securityobject so on m.SecurityObjectId = so.Id where m.ParentId = '00000000-0000-0000-0000-000000000000' or m.ParentId is null", this.TableName));
        }

        public Module FindOneByKey(string key)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>(String.Format(@"select m.*,so.Name,so.Description from {0} m left join securityobject so on m.SecurityObjectId = so.Id where m.`Key` = @Key", this.TableName), new { Key = key });
        }

        public new Module FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Module>(String.Format(@"select m.*,so.Name,so.Description from {0} m left join securityobject so on m.SecurityObjectId = so.Id where m.Id = @Id", this.TableName), new { Id = id });
        }

        public void ChangeModuleParentId(Guid parentId, IList<Guid> sourceIds)
        {
            this.UnitOfWork.Connection.Execute(String.Format(@"update {0} set ParentId=@ParentId where Id in @SourceIds", this.TableName), new { ParentId = parentId, SourceIds = sourceIds.ToArray() });
        }

        protected override string GetSaveSegmentSql()
        {
            return "`Key`,ParentId,SecurityObjectId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "`Key`,ParentId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new System.NotImplementedException();
        }
    }
}
