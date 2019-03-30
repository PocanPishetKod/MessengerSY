using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Contact
{
    public class SyncContactsResponse
    {
        public int AddContactsCount { get; set; }
        public IEnumerable<ContactModel> AddContacts { get; set; }
    }
}
