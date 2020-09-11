using KeePass.DataExchange;
using KeePassLib.Utility;
using System;
using System.Net;
using System.Threading.Tasks;

namespace KeeAnonAddy
{
    internal class AnonAddyService
    {
        private readonly Func<string> accessTokenFactory;

        enum AnonAddyRequestType
        {
            Uuid
        }

        public AnonAddyService(Func<string> accessTokenFactory)
        {
            this.accessTokenFactory = accessTokenFactory;
        }

        internal async Task<AnonAddyEntry> RequestAddress(string description)
        {
            var client = new WebClient();

            var accessToken = accessTokenFactory.Invoke();

            const string url = "https://app.anonaddy.com/api/v1/aliases";
            string entryDescription = string.IsNullOrWhiteSpace(description)
                ? "Created from KeePass."
                : description;

            string body = TemplateRequest(AnonAddyRequestType.Uuid, entryDescription);

            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            client.Headers[HttpRequestHeader.Authorization] = $"Bearer {accessToken}";
            client.Headers.Add("X-Requested-With", "XMLHttpRequest");

            string responseBody = await client.UploadStringTaskAsync(new Uri(url), body);

            Guid? id = GetIdFrom(responseBody);
            string address = GetAddressFrom(responseBody);

            return new AnonAddyEntry(id, address);
        }

        private Guid? GetIdFrom(string responseBody)
        {
            var charStream = new CharStream(responseBody);
            var json = new JsonObject(charStream);

            var data = json.GetValue<JsonObject>("data");

            var id = data.GetValue<string>("id");

            if (Guid.TryParse(id, out var uuid))
            {
                return uuid;
            }

            return null;
        }

        private string GetAddressFrom(string responseBody)
        {
            var charStream = new CharStream(responseBody);
            var json = new JsonObject(charStream);

            var data = json.GetValue<JsonObject>("data");

            return data.GetValue<string>("email");
        }

        private string TemplateRequest(AnonAddyRequestType requestType, string description)
        {
            switch (requestType)
            {
                default:
                    return $"{{\"domain\":\"anonaddy.me\",\"description\":\"{description}\",\"format\":\"uuid\"}}";
            }

        }
    }
}