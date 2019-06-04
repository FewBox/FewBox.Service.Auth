using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IApiRepository : IBaseRepository<Api, Guid>
    {
        Api FindOneByServiceAndControllerAndAction(string service, string controller, string action);
        IEnumerable<Api> FindAllByKeyword(string keyword);
    }
}
