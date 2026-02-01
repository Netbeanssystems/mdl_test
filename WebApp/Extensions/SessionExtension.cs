using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApp.Extensions
{
    public static class SessionExtension
    {
        public static void SetObjectToSession(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            }));
        }

        public static T GetObjectFromSession<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
