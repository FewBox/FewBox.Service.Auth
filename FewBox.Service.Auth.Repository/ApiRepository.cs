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
            return this.UnitOfWork.Connection.Query<Api>($"select * from {this.TableName} where Controller like @Controller", new { Controller = "%" + keyword + "%"});
        }

        public Api FindOneByServiceAndControllerAndAction(string service, string controller, string action)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Api>($"select * from {this.TableName} where Controller=@Controller and Action=@Action and SecurityObjectId in (select Id from securityobject where ServiceId = (select Id from service where Name=@Service))", 
                new { Controller = controller, Action = action, Service = service });
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
