namespace Rolls.Callers
{
    public class DepartmentCaller : IDepartmentCaller
    {
        private readonly RestClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly string _apiKey;

        public DepartmentCaller(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
            _url = _configuration.GetValue<string>("ApiUrls:Rolls");
            _client = new RestClient(_url);
        }

        public async Task<string> DepartmentAddAsync(DepartmentAddEditVM dataObj)
        {
            var request = new RestRequest("department/add", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DepartmentNameExistsAsync(IsExistsVM dataObj)
        {
            var request = new RestRequest("department/name/exists", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetDepartmentsAsync()
        {
            var request = new RestRequest($"department/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }
    }
}
