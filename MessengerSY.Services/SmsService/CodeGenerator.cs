using MessengerSY.Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MessengerSY.Services.SmsService
{
    public static class CodeGenerator
    {
        public static string GenerateCode(int count = HelpSmsConstants.DIGITS_COUNT)
        {
            var code = string.Empty;
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                code += random.Next(0, 10);
            }

            return code;
        }
    }
}
