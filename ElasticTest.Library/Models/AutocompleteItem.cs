namespace ElasticTest.Library.Models;

public class AutocompleteItem
{
    public AutocompleteItem(string suggestion)
    {
        Suggest = suggestion;
    }

    public string Suggest { get; set; }
}
