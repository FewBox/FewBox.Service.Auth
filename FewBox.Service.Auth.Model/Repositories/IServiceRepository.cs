using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IServiceRepository : IBaseRepository<S.Service, Guid>
    {
    }
}
