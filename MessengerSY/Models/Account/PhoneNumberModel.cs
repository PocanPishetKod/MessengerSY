using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Account
{
    public class PhoneNumberModel
    {
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string PhoneNumber { get; set; }
    }
}
