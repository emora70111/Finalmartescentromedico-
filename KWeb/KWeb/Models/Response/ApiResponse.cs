﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Models.Response
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}