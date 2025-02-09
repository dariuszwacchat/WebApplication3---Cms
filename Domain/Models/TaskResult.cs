﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class TaskResult<T>
    {
        public T Model { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
