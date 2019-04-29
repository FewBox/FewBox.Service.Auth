﻿using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class SecurityObject : Entity<Guid>
    {
        public Guid AppId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
