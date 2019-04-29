using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Repository
{
    public class SecurityObjectRepository : BaseRepository<SecurityObject, Guid>, ISecurityObjectRepository
    {
        public SecurityObjectRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("securityobject", ormSession, currentUser)
        {
        }

        public int UpdateAppId(Guid id, Guid appId)
        {
            return this.UnitOfWork.Connection.Execute($"update {this.TableName} set AppId=@AppId where Id=@Id", new { AppId = appId, Id = id});
        }

        protected override string GetSaveSegmentSql()
        {
            return "AppId,Name,Description";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "AppId,Name,Description";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
