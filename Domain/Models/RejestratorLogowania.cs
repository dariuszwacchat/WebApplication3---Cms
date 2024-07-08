using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RejestratorLogowania
    {
        [Key]
        public string RejestratorLogowaniaId { get; set; }
        public string DataZalogowania { get; set; }
        public string DataWylogowania { get; set; }

        public string? UserId { get; set; }
        //public ApplicationUser User { get; set; }
    }
}
