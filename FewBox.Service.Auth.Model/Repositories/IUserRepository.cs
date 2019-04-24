using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
        bool IsPasswordValid(string username, string password, out Guid userId);
        bool IsPasswordValid(Guid userId, string password);
        User FindOneByUsername(string username, UserType userType);
        User FindOneByUsername(string username);
        Guid SaveWithMD5Password(User user, string password);
        bool IsExist(string email);
        void ResetPassword(string email, string password);
        void ResetPassword(Guid id, string password);
        int CountByRoleCode(string roleCode);
        IEnumerable<User> FindAllByKeyword(string keyword);
        IEnumerable<User> FindAllByIds(Guid[] ids);
        IEnumerable<User> FindAllByType(UserType userType);
    }
}
