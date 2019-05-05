using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Repository
{
    public class ServiceRepository : BaseRepository<S.Service, Guid>, IServiceRepository
    {
        public ServiceRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("service", ormSession, currentUser)
        {
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name,Description";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name,Description";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
