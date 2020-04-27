using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IApiRepository : IRepository<Api>
    {
        bool IsExist(Guid serviceId, string controller, string action);
        Api FindOneByServiceAndControllerAndAction(Guid serviceId, string controller, string action);
        Api FindOneByServiceAndControllerAndAction(string service, string controller, string action);
        IEnumerable<Api> FindAllByKeyword(string keyword);
    }
}
