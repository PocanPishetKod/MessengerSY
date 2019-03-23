using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.SmsService
{
    public interface ISmsSenderService
    {
        Task<bool> SendCode(string phoneNumber, string code);
    }
}
