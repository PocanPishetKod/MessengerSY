using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core.SmsProvider
{
    public class SmsProviderInfo : ISmsProviderInfo
    {
        public SmsProviderInfo(string authKey)
        {
            AuthKey = authKey;
        }

        public string AuthKey { get; private set; }
    }
}
