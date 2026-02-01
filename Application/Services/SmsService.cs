using Application.AppSettings;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _smsSettings;
        public SmsService(IOptions<SmsSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }
        public Task<string> SendSmsAsync(SmsVM SmsVm)
        {
            var ResponseString = SendSms(SmsVm);
            return Task.FromResult(ResponseString);
        }

        public void OnTaskCompleted(object sender, NotifierEventArgs args)
        {
            //var ResponseString = SendSms(args.SmsVm);
            SendSms(args.SmsVm);
        }

        private string SendSms(SmsVM SmsVm)
        {
            var MobileNoStr = SmsVm.MobileNos[0];
            var BaseUrl = $"{_smsSettings.Url}?username={_smsSettings.Username}&password={_smsSettings.Password}&to={MobileNoStr}&from={_smsSettings.SenderId}&text={SmsVm.MessageText}";
            if (SmsVm.MobileNos.Count > 1)
            {
                SmsVm.MobileNos = SmsVm.MobileNos.Select(x => $"91{x}").ToList();
                MobileNoStr = string.Join(",", SmsVm.MobileNos);
                BaseUrl = $"{_smsSettings.Url}?username={_smsSettings.Username}&password={_smsSettings.Password}&to={MobileNoStr}&from={_smsSettings.SenderId}&text={SmsVm.MessageText}&category=bulk";
            }
            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            var webRequest = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
            var response = client.Send(webRequest);
            var reader = new StreamReader(response.Content.ReadAsStream());
            var ResponseString = reader.ReadToEnd();
            return ResponseString;
        }
    }
}
