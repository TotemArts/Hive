using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace Hive.Endpoints.Server.Extensions
{
    public static class AmazonSecretManagerExtensions
    {
        public static async Task<T> GetValue<T>(this IAmazonSecretsManager source, string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var result = await source.GetSecretValueAsync(request);
            return JsonConvert.DeserializeObject<T>(result.SecretString);
        }
    }
}
