using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Utils
{
    public static class Global
    {
        public static int BlockDurationMinutes = 30;

        public enum DaysOfWeek
        {
            Lunes = 1,     
            Martes = 2,   
            Miércoles = 3,  
            Jueves = 4,   
            Viernes = 5, 
            Sábado = 6,   
            Domingo = 7   
        }

        public static string GenerateRandomNumber()
        {
            Random random = new Random();
            double numeroAleatorio = random.NextDouble() * 10000000000;
            return ((long)numeroAleatorio).ToString(); 
        }


    }
}