using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class Group_UsersController : MapperController
    {
        private IGroup_UserRepository Group_UserRepository { get; set; }

        public Group_UsersController(IGroup_UserRepository group_UserRepository, IMapper mapper) : base(mapper)
        {
            this.Group_UserRepository = group_UserRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<Group_UserDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<Group_UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Group_User>, IEnumerable<Group_UserDto>>(this.Group_UserRepository.FindAll())
            };
        }

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<Group_UserDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<Group_UserDto>>
            {
                Payload = new PagingDto<Group_UserDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Group_User>, IEnumerable<Group_UserDto>>(this.Group_UserRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.Group_UserRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<Group_UserDto> Get(Guid id)
        {
            return new PayloadResponseDto<Group_UserDto>
            {
                Payload = this.Mapper.Map<Group_User, Group_UserDto>(this.Group_UserRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]Group_UserPersistantDto group_UserDto)
        {
            var group_User = this.Mapper.Map<Group_UserPersistantDto, Group_User>(group_UserDto);
            Guid group_UserId = this.Group_UserRepository.Save(group_User);
            return new PayloadResponseDto<Guid> {
                Payload = group_UserId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]Group_UserPersistantDto group_UserDto)
        {
            var group_User = this.Mapper.Map<Group_UserPersistantDto, Group_User>(group_UserDto);
            group_User.Id = id;
            this.Group_UserRepository.Update(group_User);
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            this.Group_UserRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }
    }
}
