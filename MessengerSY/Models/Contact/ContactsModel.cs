using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Contact
{
    public class ContactsModel
    {
        [Required]
        public IEnumerable<ContactModel> Contacts { get; set; }
    }
}
