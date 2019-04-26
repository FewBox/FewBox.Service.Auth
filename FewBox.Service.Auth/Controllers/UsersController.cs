using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class UsersController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private ILDAPService LDAPService { get; set; }

        public UsersController(IUserRepository userRepository, IPrincipalRepository principalRepository, 
            IPrincipal_RoleRepository principal_RoleRepository, ILDAPService ldapService, IMapper mapper): base(mapper)
        {
            this.UserRepository = userRepository;
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.LDAPService = ldapService;
        }

        [HttpGet("search/{keyword}")]
        public PayloadResponseDto<IEnumerable<UserDto>> GetByKeyword(string keyword)
        {
            return new PayloadResponseDto<IEnumerable<UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAllByKeyword(keyword))
            };
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<UserDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAll())
            };
        }

        [HttpGet("paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<UserDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<UserDto>>
            {
                Payload = new PagingDto<UserDto>
                {
                    Items = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.UserRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<UserDto> Get(Guid id)
        {
            return new PayloadResponseDto<UserDto>
            {
                Payload = this.Mapper.Map<User, UserDto>(this.UserRepository.FindOne(id))
            };
        }

        [HttpGet("ids")]
        public PayloadResponseDto<IEnumerable<UserProfileDto>> GetByIds(Guid[] ids)
        {
            return new PayloadResponseDto<IEnumerable<UserProfileDto>> {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserProfileDto>>(this.UserRepository.FindAllByIds(ids))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]UserPersistantDto userDto)
        {
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            var queryUser = this.UserRepository.FindOneByUsername(user.Name, user.Type);
            if(queryUser != null){
                return new PayloadResponseDto<Guid> { IsSuccessful = false, ErrorCode="USERNAME_ALREADYEXISTS", ErrorMessage ="The user name is already exits!", Payload = Guid.Empty };
            }
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            user.PrincipalId = principalId;
            Guid userId = this.UserRepository.SaveWithMD5Password(user, userDto.Password);
            if (userDto.RoleIds != null)
            {
                foreach (Guid roleId in userDto.RoleIds)
                {
                    this.Principal_RoleRepository.Save(new Principal_Role {
                        PrincipalId = principalId,
                        RoleId = roleId
                    });
                }
            }
            return new PayloadResponseDto<Guid> {
                Payload = userId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]UserPersistantDto userDto)
        {
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            user.Id = id;
            var updateUser = this.UserRepository.FindOne(id);
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.Id = updateUser.PrincipalId;
            this.PrincipalRepository.Update(principal);
            this.UserRepository.Update(user);
            this.Principal_RoleRepository.DeleteByPrincipalId(principal.Id);
            if (userDto.RoleIds != null)
            {
                foreach (Guid roleId in userDto.RoleIds)
                {
                    this.Principal_RoleRepository.Save(new Principal_Role
                    {
                        PrincipalId = principal.Id,
                        RoleId = roleId
                    });
                }
            }
            return new MetaResponseDto();
        }

        [HttpPut("sync/{id}")]
        [Transaction]
        public MetaResponseDto Sync(Guid id)
        {
            this.LDAPService.SyncUserProfile(id);
            return new MetaResponseDto();
        }

        [HttpPut("syncall")]
        [Transaction]
        public MetaResponseDto SyncAll()
        {
            this.LDAPService.SyncAllUserProfiles();
            return new MetaResponseDto();
        }

        [HttpPut("resetpassword/{id}")]
        [Transaction]
        public MetaResponseDto ResetPassword(Guid id, [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            this.UserRepository.ResetPassword(id, resetPasswordRequestDto.Password);
            return new MetaResponseDto { };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            var updateUser = this.UserRepository.FindOne(id);
            this.PrincipalRepository.Recycle(updateUser.PrincipalId);
            this.UserRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }

        [HttpGet("count")]
        public PayloadResponseDto<int> GetTotalNumber()
        {
            return new PayloadResponseDto<int> {
                Payload = this.UserRepository.Count()
            };
        }

        [HttpGet("count/rolecode/{roleCode}")]
        public PayloadResponseDto<int> GetTotalNumberByRoleCode(string roleCode)
        {
            return new PayloadResponseDto<int> {
                Payload = this.UserRepository.CountByRoleCode(roleCode)
            };
        }
    }
}
