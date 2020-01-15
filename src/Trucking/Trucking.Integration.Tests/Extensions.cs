using Moq;
using Newtonsoft.Json;
using System.Net.Http;

namespace Trucking.Integration.Tests
{
    internal static class Extensions
    {
        internal static Mock<T> AsMock<T>(this T obj) where T : class
        {
            return Mock.Get(obj);
        }

        internal static HttpResponseMessage AsHttpResponseMessage(this object obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(serialized)
            };
        }
    }
}
