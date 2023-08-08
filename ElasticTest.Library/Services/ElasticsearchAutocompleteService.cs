using ElasticTest.Library.Models;
using Nest;

namespace ElasticTest.Library.Services;

public class ElasticsearchAutocompleteService
{
    private const string IndexName = "autocomplete_index";
    private const string EdgeNgramTokenizerName = "edge_ngram";
    private const string EdgeNgramAnalyzerName = "autocomplete_analyzer";

    private readonly IElasticClient _elasticClient;

    public ElasticsearchAutocompleteService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void CreateIndex()
    {
        if (_elasticClient.Indices.Exists(IndexName).Exists)
        {
            _elasticClient.Indices.Delete(IndexName);
            Console.WriteLine("Deleted existing index");
        }

        _elasticClient.Indices.Create(IndexName, createIndexDescriptor => createIndexDescriptor.Settings(descriptor =>
                descriptor.Analysis(analysis => analysis
                    .Tokenizers(tokenizer =>
                        tokenizer.EdgeNGram(EdgeNgramTokenizerName, ngram => ngram.MinGram(1).MaxGram(25)))
                    .Analyzers(analyzer =>
                        analyzer.Custom(EdgeNgramAnalyzerName, custom => custom.Tokenizer(EdgeNgramTokenizerName).Filters("lowercase")))))
            .Map<AutocompleteItem>(mapping => mapping
                .Properties(p => p.Text(descriptor => descriptor.Name(item => item.Suggest).Analyzer(EdgeNgramAnalyzerName)))
            ));
    }

    public void IndexData(IEnumerable<string> words)
    {
        var data = words.Select(word => new AutocompleteItem(word));
        _elasticClient.IndexMany(data, IndexName);
    }

    public IEnumerable<string> GetAutocompleteSuggestions(string query, int size)
    {
        int numberOfPossibleErrors = query.Length > 7 ? 1 : 0; // Allows error in the last character in addition of 2 errors allowed by Fuzziness.
        var analyzer = numberOfPossibleErrors == 0 ? "standard" : EdgeNgramAnalyzerName;
        var fuziness = numberOfPossibleErrors > 0 
            ? Fuzziness.EditDistance(3 - numberOfPossibleErrors) // to ensure small tokens can also have errors.
            : Fuzziness.Auto;

        // Use Fixed as the number of tokens (clauses) equals the number of characters.
        // Percentage could be used as well.
        var minimumShouldMatch = MinimumShouldMatch.Fixed(-numberOfPossibleErrors); 

        var searchResponse = _elasticClient.Search<AutocompleteItem>(s => s
            .Index(IndexName)
            .Query(queryDescriptor => queryDescriptor
                .Match(c => c
                    .Field(item => item.Suggest)
                    .Query(query)
                    .Analyzer(analyzer)
                    .Fuzziness(fuziness) // Allows errors in the beginning/middle of the word.
                    .MinimumShouldMatch(minimumShouldMatch)
                )
            ).Size(size)
        );

        var options = searchResponse.Documents
            .Select(x => x.Suggest);

        return options;
    }
}