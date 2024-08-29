namespace Rolls.Callers
{
  public class CountryCaller : ICountryCaller
  {
    private readonly RestClient _client;
    private readonly IConfiguration _configuration;
    private readonly string _url;
    private readonly string _apiKey;

    public CountryCaller(IConfiguration configuration)
    {
      _configuration = configuration;
      _apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
      _url = _configuration.GetValue<string>("ApiUrls:Rolls");
      _client = new RestClient(_url);
    }

    public async Task<string> CountryNameExistsAsync(IsExistsVM dataObj)
    {
      var request = new RestRequest("country/name/exists", Method.Post);
      request.AddHeader("Api-Key", _apiKey);
      request.RequestFormat = DataFormat.Json;
      request.AddBody(dataObj);
      var result = await _client.ExecuteAsync<string>(request);

      return result.Content;
    }

    public async Task<string> GetCountriesAsync()
    {
      var request = new RestRequest($"country/all", Method.Get);
      request.AddHeader("Api-Key", _apiKey);
      var result = await _client.ExecuteAsync<string>(request);

      return result.Content;
    }


  }
}
