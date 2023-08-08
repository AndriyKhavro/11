using ElasticTest.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticTest.Controllers;

[ApiController]
[Route("api/autocomplete")]
public class AutocompleteController : ControllerBase
{
    private readonly ElasticsearchAutocompleteService _autocompleteService;

    public AutocompleteController(ElasticsearchAutocompleteService autocompleteService)
    {
        _autocompleteService = autocompleteService;
    }

    [HttpGet]
    [Route("suggestions")]
    public ActionResult<IEnumerable<string>> GetAutocompleteSuggestions(string q, int size = 10)
    {
        if (string.IsNullOrEmpty(q))
        {
            return BadRequest("The 'q' parameter cannot be empty.");
        }

        var suggestions = _autocompleteService.GetAutocompleteSuggestions(q, size);

        return Ok(suggestions);
    }
}