using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ChangeEmailViewModel
    {
        public string UserName { get; set; }

        public string ObecnyEmail { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; } 
    }
}
