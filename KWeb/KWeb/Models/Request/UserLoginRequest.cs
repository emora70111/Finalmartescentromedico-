﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Models.Request
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Rememberme{ get; set; }
    }
}