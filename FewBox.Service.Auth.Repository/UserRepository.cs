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
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id where principal.Name like @Name", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Name = "%" + keyword + "%" });
        }

        public new User FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id having {0}.Id = @Id", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Id = id }).SingleOrDefault();
        }

        public new IEnumerable<User> FindAll()
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id", this.TableName), 
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; });
        }

        public new IEnumerable<User> FindAll(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id limit @From,@PageRange", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { From = from, PageRange = pageRange });
        }

        public IEnumerable<User> FindAllByIds(Guid[] ids)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id where {0}.Id in @Ids ", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Ids = ids });
        }

        public User FindOneByUsername(string username, UserType userType)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id  having {0}.Type = @Type and PrincipalId in (select Id from principal where Name = @Name)", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Name = username, Type = userType }).SingleOrDefault();
        }

        public User FindOneByUsername(string username)
        {
            return this.FindOneByUsername(username, UserType.Form);
        }

        public User FindOneByPrincipalId(Guid principalId)
        {
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id where PrincipalId = (select Id from principal where PrincipalId = @PrincipalId)", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { PrincipalId = principalId }).SingleOrDefault();
        }

        public bool IsExist(string email)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format(@"select count(1) from {0} where Email=@Email", this.TableName), new { Email = email }) > 0;
        }

        public bool IsPasswordValid(string username, string password)
        {
            bool isPasswordValid = false;
            var user = this.FindOneByUsername(username);
            if (user != null)
            {
                isPasswordValid = user.SaltMD5Password == SaltMD5Utility.Encrypt(password, user.Salt.ToString());
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
            this.UnitOfWork.Connection.Execute(String.Format(@"update {0} set SaltMD5Password=@SaltMD5Password,Salt=@Salt where Email=@Email", this.TableName),
                new { SaltMD5Password = saltMD5Password, Salt = salt, Email = email });
        }

        public void ResetPassword(Guid id, string password)
        {
            Guid salt = Guid.NewGuid();
            string saltMD5Password = SaltMD5Utility.Encrypt(password, salt.ToString());
            this.UnitOfWork.Connection.Execute(String.Format(@"update {0} set SaltMD5Password=@SaltMD5Password,Salt=@Salt where Id=@Id", this.TableName),
                new { SaltMD5Password = saltMD5Password, Salt = salt, Id = id });
        }

        public Guid SaveWithMD5Password(User user, string password)
        {
            user.Id = Guid.NewGuid();
            user.Salt = Guid.NewGuid();
            user.SaltMD5Password = SaltMD5Utility.Encrypt(password, user.Salt.ToString());
            this.InitUpdateDefaultProperty(user);
            var newSegments = from column in this.GetSaveSegmentSql().Split(',')
                              select String.Format(@"@{0}", column.Trim());
            string sql = String.Format(@"insert into {0} ({1} Salt, SaltMD5Password, Id, CreatedTime, ModifiedTime, CreatedBy, ModifiedBy) values({2}, @Salt, @SaltMD5Password, @Id, @CreatedTime, @ModifiedTime, @CreatedBy, @ModifiedBy)",
                this.TableName, this.GetSaveSegmentSql() + ",", String.Join(",", newSegments));
            this.UnitOfWork.Connection.Execute(sql, user);
            return user.Id;
        }

        public int CountByRoleCode(string roleCode)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format(@"select count(1) from principal_role where RoleId in (select Id from role where Code=@RoleCode) and PrincipalId in (select Id from principal where PrincipalType=0)", this.TableName),
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
            return this.UnitOfWork.Connection.Query<User, Principal, User>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id where {0}.Type = @Type ", this.TableName),
                (user, principal) => { user.Name = principal.Name; user.Description = principal.Description; return user; },
                new { Type = userType });
        }
    }
}
