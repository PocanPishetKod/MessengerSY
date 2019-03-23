using MessengerSY.Core;
using MessengerSY.Core.SmsProvider;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessengerSY.Services.SmsService
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly ISmsProviderInfo _smsProviderInfo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        private readonly string _smsSendUri = "sms/send";

        public SmsSenderService(ISmsProviderInfo smsProviderInfo, IHttpClientFactory httpClientFactory)
        {
            _smsProviderInfo = smsProviderInfo;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("sms.ru");
        }

        public async Task<bool> SendCode(string phoneNumber, string code)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            var request = CreateSmsSendRequest(phoneNumber, code);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();

            return IsMessageSend(content);
        }

        private bool IsMessageSend(string content)
        {
            var jsonObj = JObject.Parse(content);
            return string.Equals((string)jsonObj["status"], HelpSmsConstants.SUCCESS_RESPONSE);
        }

        private HttpRequestMessage CreateSmsSendRequest(string phoneNumber, string code, int json = 1)
        {
            return new HttpRequestMessage(HttpMethod.Get,
                _smsSendUri + $"?api_id={_smsProviderInfo.AuthKey}&to={phoneNumber}&msg={HelpSmsConstants.MESSAGE_TEXT}:+{code}&json={json}");
        }
    }
}
