using ElasticTest.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticTest.Library;

public static class DependencyInjection
{
    public static IServiceCollection AddElasticSearchAutocompleteService(this IServiceCollection services, string elasticSearchUrl)
    {
        var elasticConnectionSettings = new ConnectionSettings(new Uri(elasticSearchUrl))
            .ThrowExceptions();

        services.AddSingleton<IElasticClient>(_ => new ElasticClient(elasticConnectionSettings));
        services.AddTransient<ElasticsearchAutocompleteService>();
        return services;
    }
}