using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Email { get; set; }


        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } 
    }
}
