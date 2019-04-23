using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class ApiRepository : BaseRepository<Api, Guid>, IApiRepository
    {
        public ApiRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("api", ormSession, currentUser)
        {
        }

        public IEnumerable<Api> FindAllByKeyword(string keyword)
        {
            return this.UnitOfWork.Connection.Query<Api>(String.Format(@"select * from {0} where Controller like @Controller", this.TableName), new { Controller = "%" + keyword + "%"});
        }

        public Api FindOneByControllerAndAction(string controller, string action)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Api>(String.Format(@"select * from {0} where Controller=@Controller and Action=@Action", this.TableName), 
                new { Controller = controller, Action = action });
        }

        protected override string GetSaveSegmentSql()
        {
            return "Controller,Action,SecurityObjectId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Controller,Action";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
