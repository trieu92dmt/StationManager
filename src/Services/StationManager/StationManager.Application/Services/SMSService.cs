using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace StationManager.Application.Services
{
    public interface ISMSService
    {
        bool SendSMS(string body, string to);
    }

    public class SMSService : ISMSService
    {
        private readonly IConfiguration _config;

        public SMSService(IConfiguration config)
        {
            _config = config;
        }

        public bool SendSMS(string body, string to)
        {
            //Get Account
            var account = _config.GetSection("Twilio")["Account"];
            var authTokent = _config.GetSection("Twilio")["AuthToken"];
            var phoneNumber = _config.GetSection("Twilio")["PhoneNumber"];

            TwilioClient.Init(account, authTokent);

            var message = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber(phoneNumber),
                to: new Twilio.Types.PhoneNumber(to)
            );

            return true;
        }
    }
}
