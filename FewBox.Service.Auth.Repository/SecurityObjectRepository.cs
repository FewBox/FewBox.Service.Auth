using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Repository
{
    public class SecurityObjectRepository : BaseRepository<SecurityObject, Guid>, ISecurityObjectRepository
    {
        public SecurityObjectRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("securityobject", ormSession, currentUser)
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
