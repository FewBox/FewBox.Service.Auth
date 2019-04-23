using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Services
{
    public interface ILDAPService
    {
        bool IsPasswordValid(string userLoginName, string password);
        void SyncUserProfile(Guid id);
        void SyncUserProfiles(IList<Guid> ids);
        void SyncAllUserProfiles();
    }
}
