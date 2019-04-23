using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class GroupRepository : BaseRepository<Group, Guid>, IGroupRepository
    {
        public GroupRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("`group`", ormSession, currentUser)
        {
        }

        public new Group FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id having {0}.Id = @Id", this.TableName),
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; },
                new { Id = id }).SingleOrDefault();
        }

        public new IEnumerable<Group> FindAll()
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id", this.TableName),
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; });
        }

        public IEnumerable<Group> FindAllByRoot()
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id having {0}.PrincipalId='00000000-0000-0000-0000-000000000000'", this.TableName),
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; });
        }

        public new IEnumerable<Group> FindAll(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>(String.Format(@"select * from {0} left join principal on {0}.PrincipalId = principal.Id limit @From,@PageRange", this.TableName),
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; },
                new { From = from, PageRange = pageRange });
        }

        public int CountByRoleCode(string roleCode)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>(String.Format(@"select count(1) from principal_role where RoleId in (select Id from role where Code=@RoleCode) and PrincipalId in (select Id from principal where PrincipalType=1)", this.TableName),
                new { RoleCode = roleCode });
        }

        protected override string GetSaveSegmentSql()
        {
            return "ParentId,PrincipalId";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "ParentId";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}
