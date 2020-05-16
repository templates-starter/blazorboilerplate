﻿using System;
using System.Collections.Generic;

namespace BlazorBoilerplate.Shared.Interfaces
{
    public interface IUserSession
    {
        Guid UserId { get; set; }
        string TenantId { get; set; }
        List<string> Roles { get; set; }
        string UserName { get; set; }
        bool DisableTenantFilter { get; set; }
    }
}