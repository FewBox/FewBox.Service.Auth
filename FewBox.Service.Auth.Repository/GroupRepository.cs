using Dapper;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FewBox.Service.Auth.Repository
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser) 
        : base("`group`", ormSession, currentUser)
        {
        }

        public new Group FindOne(Guid id)
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id having {this.TableName}.Id = @Id",
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; },
                new { Id = id }).SingleOrDefault();
        }

        public Group FindOneByName(string name)
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id having principal.Name = @Name",
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; },
                new { Name = name }).SingleOrDefault();
        }

        public new IEnumerable<Group> FindAll()
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id",
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; });
        }

        public IEnumerable<Group> FindAllByRoot()
        {
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id having {this.TableName}.ParentId='00000000-0000-0000-0000-000000000000'",
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; });
        }

        public new IEnumerable<Group> FindAll(int pageIndex, int pageRange)
        {
            int from = (pageIndex - 1) * pageRange;
            return this.UnitOfWork.Connection.Query<Group, Principal, Group>($"select * from {this.TableName} left join principal on {this.TableName}.PrincipalId = principal.Id limit @From,@PageRange",
                (group, principal) => { group.Name = principal.Name; group.Description = principal.Description; return group; },
                new { From = from, PageRange = pageRange });
        }

        public int UpdateParent(Guid id, Guid parentId)
        {
            return this.UnitOfWork.Connection.Execute($"update {this.TableName} set ParentId=@ParentId where Id=@Id", new { ParentId = parentId, Id = id });
        }

        public int CountByRoleCode(string roleCode)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<int>($"select count(1) from principal_role where RoleId in (select Id from role where Code=@RoleCode) and PrincipalId in (select Id from principal where PrincipalType=1)",
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
