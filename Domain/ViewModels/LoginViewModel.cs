using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; } 
        public string? Token { get; set; }
        public string? Role { get; set; }
        //public List<string>? Role { get; set; }
    }
}
