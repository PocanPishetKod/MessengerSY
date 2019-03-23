using MessengerSY.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Account
{
    public class VerifyCodeModel
    {
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(HelpSmsConstants.DIGITS_COUNT, MinimumLength = HelpSmsConstants.DIGITS_COUNT)]
        public string Code { get; set; }
    }
}
