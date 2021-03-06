﻿using System;
using System.Collections.Generic;
using XamlingCore.Portable.Model.Contract;

namespace XamlingCore.Portable.Model.Security
{
    public class XSecurityContext : IEntity
    {
        public Guid Id { get; set; }
        public List<Guid> Targets { get; set; }
        //public Guid ParentId { get; set; }
        public List<Guid> Children { get; set; }
        public List<Guid> Members { get; set; }
        public int Permissions { get; set; }
        public string Name { get; set; }
    }
}