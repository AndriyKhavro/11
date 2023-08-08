// Build the configuration

using ElasticTest.DataGenerator;
using ElasticTest.Library;
using ElasticTest.Library.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Read the Elasticsearch URL from the configuration
var elasticsearchUrl = configuration["Elasticsearch:Url"]!;

var serviceProvider = new ServiceCollection()
    .AddSingleton(configuration)
    .AddElasticSearchAutocompleteService(elasticsearchUrl)
    .AddTransient<EnglishWordsProvider>()
    .BuildServiceProvider();

var wordsProvider = serviceProvider.GetRequiredService<EnglishWordsProvider>();
Console.WriteLine("Started fetching words");

var words = await wordsProvider.GetWords();
Console.WriteLine("Words fetched");

var autocompleteService = serviceProvider.GetRequiredService<ElasticsearchAutocompleteService>();
autocompleteService.CreateIndex();

Console.WriteLine("Index created");


autocompleteService.IndexData(words);
Console.WriteLine("Words indexed");
