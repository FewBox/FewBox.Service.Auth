using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface ISecurityObjectRepository : IRepository<SecurityObject>
    {
        bool IsExist(Guid serviceId, string name);
        SecurityObject FindOneByServiceIdAndName(Guid serviceId, string name);
        int UpdateServiceId(Guid id, Guid serviceId);
    }
}
