using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Hive.Shared.Common.Options;

namespace Hive.Shared.Common
{
    public static class MongoServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoClient(this IServiceCollection services, string databaseName, MongoDbOptions? options = null)
        {
            return services
                .AddSingleton<IMongoClient>(provider =>
                {
                    options ??= provider.GetService<IOptions<MongoDbOptions>>()?.Value ?? new MongoDbOptions();

                    var settings = MongoClientSettings.FromUrl(MongoUrl.Create(options.Url));
                    settings.UseTls = false;

                    if (!string.IsNullOrEmpty(options.Password))
                    {
                        settings.Credential = MongoCredential.CreateCredential("admin", options.Username, options.Password);
                    }

                    return new MongoClient(settings);
                })
                .AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        }
    }
}