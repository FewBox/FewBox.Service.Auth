using System;
using System.Collections.Generic;
using AutoMapper;
using FewBox.Core.Web.Security;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using Microsoft.AspNetCore.Http;

namespace FewBox.Service.Auth.Domain
{
    public class LDAPService : ILDAPService
    {
        public bool IsPasswordValid(string userLoginName, string password)
        {
            throw new NotImplementedException();
        }

        public void SyncAllUserProfiles()
        {
            throw new NotImplementedException();
        }

        public void SyncUserProfile(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SyncUserProfiles(IList<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}