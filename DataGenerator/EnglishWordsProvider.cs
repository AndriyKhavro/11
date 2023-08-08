namespace ElasticTest.DataGenerator;

internal class EnglishWordsProvider
{
    private const string Url = "https://raw.githubusercontent.com/dwyl/english-words/master/words.txt";

    public async Task<string[]> GetWords()
    {
        using var httpClient = new HttpClient();

        var fileContent = await httpClient.GetStringAsync(Url);

        return fileContent.Split('\n');
    }
}
