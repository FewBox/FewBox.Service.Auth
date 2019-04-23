using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface ISecurityObjectRepository : IBaseRepository<SecurityObject, Guid>
    {
    }
}
