﻿using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Tenant FindOneByName(string name);
        bool IsExist(string name);
    }
}