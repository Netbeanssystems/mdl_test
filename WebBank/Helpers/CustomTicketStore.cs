using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebBank.Helpers
{
    public class CustomTicketStore : ITicketStore
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _config;

        public CustomTicketStore(IServiceCollection services, IConfiguration config)
        {
            _services = services;
            _config = config;
        }

        public async Task RemoveAsync(string key)
        {
            using var scope = _services.BuildServiceProvider().CreateScope();
            var _httpClient = scope.ServiceProvider.GetService<IHttpClientService>();
            var TicketResult = await _httpClient.GetAsync("AuthenticationTicketsBank/Get", false, key).ConfigureAwait(false);
            var ticket = !string.IsNullOrEmpty(TicketResult) ? JsonConvert.DeserializeObject<AuthenticationTicketsBankDTO>(TicketResult) : null;
            if (ticket != null)
                await _httpClient.DeleteAsync("AuthenticationTicketsBank/Delete", false, key).ConfigureAwait(false);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            using var scope = _services.BuildServiceProvider().CreateScope();
            var _httpClient = scope.ServiceProvider.GetService<IHttpClientService>();
            var TicketResult = await _httpClient.GetAsync("AuthenticationTicketsBank/Get", false, key).ConfigureAwait(false);
            var authenticationTicket = !string.IsNullOrEmpty(TicketResult) ? JsonConvert.DeserializeObject<AuthenticationTicketsBankDTO>(TicketResult) : null;
            if (authenticationTicket != null)
            {
                authenticationTicket.Value = SerializeToBytes(ticket);
                authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                authenticationTicket.Expires = ticket.Properties.ExpiresUtc;
                await _httpClient.PutAsync("AuthenticationTicketsBank/Edit", false, key, authenticationTicket).ConfigureAwait(false);
            }
        }

        [Obsolete]
        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            using var scope = _services.BuildServiceProvider().CreateScope();
            var _httpClient = scope.ServiceProvider.GetService<IHttpClientService>();
            var TicketResult = await _httpClient.GetAsync("AuthenticationTicketsBank/Get", false, key).ConfigureAwait(false);
            var authenticationTicket = !string.IsNullOrEmpty(TicketResult) ? JsonConvert.DeserializeObject<AuthenticationTicketsBankDTO>(TicketResult) : null;
            if (authenticationTicket != null)
            {
                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext != null)
                {
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
                    var userAgent = httpContext.Request.Headers["User-Agent"];
                    if (!string.IsNullOrEmpty(userAgent))
                    {
                        var uaParser = UAParser.Parser.GetDefault();
                        var clientInfo = uaParser.Parse(userAgent);
                        if (authenticationTicket.RemoteIpAddress != remoteIpAddress.ToString() || authenticationTicket.UserAgentFamily != clientInfo.UserAgent.Family)
                            return null;
                        authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                        authenticationTicket.Expires = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_config["CookieExpiry"]));
                        await _httpClient.PutAsync("AuthenticationTicketsBank/Edit", false, key, authenticationTicket).ConfigureAwait(false);
                        return DeserializeFromBytes(authenticationTicket.Value);
                    }
                }
            }
            return null;
        }

        [Obsolete]
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var userId = string.Empty;
            var userName = string.Empty;
            if (ticket.AuthenticationScheme == "Cookies")
            {
                userId = ticket.Principal.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (ticket.Principal.Identity != null) userName = ticket.Principal.Identity.Name;
            }

            var authenticationTicket = new AuthenticationTicketsBankDTO
            {
                Id = userId,
                UserId = userId,
                UserName = userName,
                LastActivity = DateTimeOffset.UtcNow,
                Value = SerializeToBytes(ticket)
            };

            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                authenticationTicket.Expires = expiresUtc.Value;
            }

            using var scope = _services.BuildServiceProvider().CreateScope();
            var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    authenticationTicket.RemoteIpAddress = remoteIpAddress.ToString();
                }

                var userAgent = httpContext.Request.Headers["User-Agent"];
                if (!string.IsNullOrEmpty(userAgent))
                {
                    var uaParser = UAParser.Parser.GetDefault();
                    var clientInfo = uaParser.Parse(userAgent);
                    authenticationTicket.OperatingSystem = clientInfo.OS.ToString();
                    authenticationTicket.UserAgentFamily = clientInfo.UserAgent.Family;
                    authenticationTicket.UserAgentVersion = $"{clientInfo.UserAgent.Major}.{clientInfo.UserAgent.Minor}.{clientInfo.UserAgent.Patch}";
                }
            }

            var _httpClient = scope.ServiceProvider.GetService<IHttpClientService>();
            await _httpClient.PostAsync("AuthenticationTicketsBank/Upsert", false, authenticationTicket).ConfigureAwait(false);
            return authenticationTicket.Id;
        }

        private byte[] SerializeToBytes(AuthenticationTicket source)
            => TicketSerializer.Default.Serialize(source);

        private AuthenticationTicket DeserializeFromBytes(byte[] source)
            => source == null ? null : TicketSerializer.Default.Deserialize(source);
    }
}