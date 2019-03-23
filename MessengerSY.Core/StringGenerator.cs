using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MessengerSY.Core
{
    public static class StringGenerator
    {
        public static string GenerateString(int bytesCount)
        {
            var randomNumber = new byte[bytesCount];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
