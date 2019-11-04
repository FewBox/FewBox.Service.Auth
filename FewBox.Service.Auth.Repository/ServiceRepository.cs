using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Repository
{
    public class ServiceRepository : BaseRepository<S.Service, Guid>, IServiceRepository
    {
        public ServiceRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("service", ormSession, currentUser)
        {
        }

        public S.Service FindOneByName(string name)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<S.Service>($"select * from {this.TableName} where Name = @Name",
                new { Name = name });
        }

        public bool IsExist(string name)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Name = @Name",
                new { Name = name }) > 0;
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name,Description";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name,Description";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
