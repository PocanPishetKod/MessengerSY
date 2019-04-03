using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Contact
{
    public class ContactModel
    {
        public int ContactId { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactName { get; set; }
    }
}
