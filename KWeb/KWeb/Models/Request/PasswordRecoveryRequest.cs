using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Models.Request
{
    public class PasswordRecoveryRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }

        public string NewPassword { get; set; }
    }
}