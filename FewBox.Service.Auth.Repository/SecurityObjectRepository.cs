using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Repository
{
    public class SecurityObjectRepository : Repository<SecurityObject>, ISecurityObjectRepository
    {
        public SecurityObjectRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("securityobject", ormSession, currentUser)
        {
        }

        public SecurityObject FindOneByServiceIdAndName(Guid serviceId, string name)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<SecurityObject>($"select * from {this.TableName} where ServiceId=@ServiceId and Name = @Name",
               new { ServiceId = serviceId, Name = name });
        }

        public bool IsExist(Guid serviceId, string name)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where ServiceId=@ServiceId and Name = @Name",
                new { ServiceId = serviceId, Name = name }) > 0;
        }

        public int UpdateServiceId(Guid id, Guid serviceId)
        {
            return this.UnitOfWork.Connection.Execute($"update {this.TableName} set ServiceId=@ServiceId where Id=@Id", new { ServiceId = serviceId, Id = id });
        }

        protected override string GetSaveSegmentSql()
        {
            return "ServiceId,Name,Description";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "ServiceId,Name,Description";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
