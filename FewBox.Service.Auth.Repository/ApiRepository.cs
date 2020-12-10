using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Repository
{
    public class ApiRepository : CommonRepository<Api>, IApiRepository
    {
        public ApiRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("api", ormSession, currentUser)
        {
        }

        public IEnumerable<Api> FindAllByKeyword(string keyword)
        {
            return this.UnitOfWork.Connection.Query<Api>($"select * from {this.TableName} where Controller like @Controller", new { Controller = "%" + keyword + "%" });
        }

        public IEnumerable<Api> FindAllByUserId(Guid userId)
        {
            var principalIds = this.GetPrincipalIds(userId);
            return this.UnitOfWork.Connection.Query<Api, SecurityObject, Api>(
                $@"select * from {this.TableName} left join securityobject on {this.TableName}.SecurityObjectId = securityobject.Id where SecurityObjectId in
                (select SecurityObjectId from role_security where RoleId in
                (select RoleId from principal_role where PrincipalId in @PrincipalIds))",
                (api, secuirtyObject) => { api.ServiceId = secuirtyObject.ServiceId; return api; },
                new { PrincipalIds = principalIds });
        }

        public Api FindOneByServiceAndControllerAndAction(Guid serviceId, string controller, string action)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Api>($"select * from {this.TableName} where Controller=@Controller and Action=@Action and SecurityObjectId in (select Id from securityobject where ServiceId = @ServiceId)",
                new { Controller = controller, Action = action, ServiceId = serviceId });
        }

        public Api FindOneByServiceAndControllerAndAction(string service, string controller, string action)
        {
            return this.UnitOfWork.Connection.QuerySingleOrDefault<Api>($"select * from {this.TableName} where Controller=@Controller and Action=@Action and SecurityObjectId in (select Id from securityobject where ServiceId = (select Id from service where Name=@Service))",
                new { Controller = controller, Action = action, Service = service });
        }

        public bool IsExist(Guid serviceId, string controller, string action)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Controller=@Controller and Action=@Action and SecurityObjectId in (select Id from securityobject where ServiceId = @ServiceId)",
                new { Controller = controller, Action = action, ServiceId = serviceId }) > 0;
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
