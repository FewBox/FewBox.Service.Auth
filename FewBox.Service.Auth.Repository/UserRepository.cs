using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Utility.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("user", ormSession, currentUser)
        {
        }

        public IEnumerable<User> FindAllByKeyword(string keyword)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id where principal.Name like @Name",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Name = "%" + keyword + "%" });
        }

        public new User FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id having {this.TableName}.Id = @Id",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Id = id }).SingleOrDefault();
        }

        public new IEnumerable<User> FindAll()
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; });
        }

        public new IEnumerable<User> FindAll(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id limit @From,@PageRange",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { From = from, PageRange = pageRange });
        }

        public IEnumerable<User> FindAllByIds(Guid[] ids)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id where {this.TableName}.Id in @Ids ",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Ids = ids });
        }

        public User FindOneByUsername(string username, UserType userType)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id  having {this.TableName}.Type = @Type and PrincipalId in (select Id from principal where Name = @Name)",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Name = username, Type = userType }).SingleOrDefault();
        }

        public User FindOneByUsername(string username)
        {
            return this.FindOneByUsername(username, UserType.Form);
        }

        public User FindOneByPrincipalId(Guid principalId)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id where PrincipalId = (select Id from principal where PrincipalId = @PrincipalId)",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { PrincipalId = principalId }).SingleOrDefault();
        }

        public bool IsExist(string email)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from {this.TableName} where Email=@Email", new { Email = email }) > 0;
        }

        public bool IsPasswordValid(string username, string password, out Guid userId)
        {
            bool isPasswordValid = false;
            var user = this.FindOneByUsername(username);
            if (user != null)
            {
                userId = user.Id;
                isPasswordValid = user.SaltMD5Password == SaltMD5Utility.Encrypt(password, user.Salt.ToString());
            }
            else
            {
                userId = Guid.Empty;
            }
            return isPasswordValid;
        }

        public bool IsPasswordValid(Guid userId, string password)
        {
            bool isPasswordValid = false;
            var user = this.FindOne(userId);
            if (user != null)
            {
                isPasswordValid = user.SaltMD5Password == SaltMD5Utility.Encrypt(password, user.Salt.ToString());
            }
            return isPasswordValid;
        }

        public void ResetPassword(string email, string password)
        {
            Guid salt = Guid.NewGuid();
            string saltMD5Password = SaltMD5Utility.Encrypt(password, salt.ToString());
            this.UnitOfWork.Connection.Execute($"update {this.TableName} set SaltMD5Password=@SaltMD5Password,Salt=@Salt where Email=@Email",
                new { SaltMD5Password = saltMD5Password, Salt = salt, Email = email });
        }

        public void ResetPassword(Guid id, string password)
        {
            Guid salt = Guid.NewGuid();
            string saltMD5Password = SaltMD5Utility.Encrypt(password, salt.ToString());
            this.UnitOfWork.Connection.Execute($"update {this.TableName} set SaltMD5Password=@SaltMD5Password,Salt=@Salt where Id=@Id",
                new { SaltMD5Password = saltMD5Password, Salt = salt, Id = id });
        }

        public Guid SaveWithMD5Password(User user, string password)
        {
            user.Id = Guid.NewGuid();
            user.Salt = Guid.NewGuid();
            user.SaltMD5Password = SaltMD5Utility.Encrypt(password, user.Salt.ToString());
            this.InitUpdateDefaultProperty(user);
            var newSegments = from column in this.GetSaveSegmentSql().Split(',')
                              select $"@{column.Trim()}";
            string sql = $"insert into {this.TableName} ({this.GetSaveSegmentSql()}, Salt, SaltMD5Password, Id, CreatedTime, ModifiedTime, CreatedBy, ModifiedBy) values({String.Join(",", newSegments)}, @Salt, @SaltMD5Password, @Id, @CreatedTime, @ModifiedTime, @CreatedBy, @ModifiedBy)";
            this.UnitOfWork.Connection.Execute(sql, user);
            return user.Id;
        }

        public int CountByRoleCode(string roleCode)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from principal_role where RoleId in (select Id from role where Code=@RoleCode) and PrincipalId in (select Id from principal where PrincipalType=0)",
                new { RoleCode = roleCode });
        }

        protected override string GetSaveSegmentSql()
        {
            return "DisplayName,Department,Email,Mobile,Type,PrincipalId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "DisplayName,Department,Email,Mobile,Type";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<User> FindAllByType(UserType userType)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id where {this.TableName}.Type = @Type ",
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Type = userType });
        }
    }
}
