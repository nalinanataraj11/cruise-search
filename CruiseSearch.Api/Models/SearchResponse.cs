namespace CruiseSearch.Api.Models;

public class SearchResponse
{
    public required List<CruiseResult> Results { get; set; }
    public required DateTime SearchTimestampUtc { get; set; }
}
