using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Repository
{
    public class PrincipalRepository : BaseRepository<Principal, Guid>, IPrincipalRepository
    {
        public PrincipalRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("principal", ormSession, currentUser)
        {
        }

        public bool IsExist(string name)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Name = @Name",
                new { Name = name }) > 0;
        }

        public Principal FindOneByName(string name)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Principal>($"select * from {this.TableName} where Name = @Name",
                new { Name = name });
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name,Description,PrincipalType";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name,Description,PrincipalType";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
