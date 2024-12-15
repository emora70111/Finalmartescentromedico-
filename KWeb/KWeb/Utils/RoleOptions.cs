using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Utils
{
    public class RoleOption
    {
        public string RoleName { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class RoleOptions
    {
        public static RoleOption GetRoleOptions(string rol)
        {
            var roles = new List<RoleOption>
            {
                new RoleOption
                {
                    RoleName = Enum.GetName(typeof(RolesEnum), RolesEnum.Administrator),
                    Options = new List<Option>
                    {
                        new Option{ Name = "Usuarios", Url = "/users/index"},
                        new Option{ Name = "Acerca de", Url = "/main/acercade"}
                    }
                },
                new RoleOption
                {
                    RoleName = Enum.GetName(typeof(RolesEnum), RolesEnum.Doctor),
                    Options = new List<Option>
                    {
                        new Option{ Name = "Citas", Url = "/appointments/ofdoctor"},
                        new Option{ Name = "Tu disponibilidad", Url = "/schedules/index"},
                        new Option{ Name = "Acerca de", Url = "/main/acercade"}
                    }
                },
                new RoleOption
                {
                    RoleName = Enum.GetName(typeof(RolesEnum), RolesEnum.Patient),
                    Options = new List<Option>
                    {
                        new Option{ Name = "Agendar cita", Url = "/appointments/ofpatient"},                        
                        new Option{ Name = "Historial clinico", Url = "/medicalhistory/index"},
                        new Option{ Name = "Acerca de", Url = "/main/acercade"}

                    }
                }
            };

            return roles.Find(r => r.RoleName == rol);
        }
    }
}