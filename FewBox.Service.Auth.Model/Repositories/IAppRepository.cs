using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IAppRepository : IBaseRepository<App, Guid>
    {
    }
}
