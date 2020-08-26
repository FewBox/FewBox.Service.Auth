using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using Dapper;

namespace FewBox.Service.Auth.Repository
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        public TenantRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("tenant", ormSession, currentUser)
        {
        }

        public Tenant FindOneByName(string name)
        {
            return this.UnitOfWork.Connection.QueryFirstOrDefault<Tenant>($"select * from {this.TableName} where Name=@Name", new { Name = name });
        }

        public bool IsExist(string name)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Name=@Name", new { Name = name }) > 0;
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
