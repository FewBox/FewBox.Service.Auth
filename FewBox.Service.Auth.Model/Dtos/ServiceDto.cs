﻿using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ServiceDto : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
