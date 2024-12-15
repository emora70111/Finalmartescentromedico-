using KWeb.Models.Request;
using KWeb.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.ViewModels
{
    public class UsersViewModel
    {
        public List<Users> Users { get; set; }
        public UserRequest UserRequest { get; set; }
    }
}