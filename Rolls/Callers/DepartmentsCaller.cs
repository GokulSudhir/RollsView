

namespace Rolls.Callers
{
    public class DepartmentsCaller : IDepartmentsCaller
    {
        private readonly RestClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly string _apiKey;

        public DepartmentsCaller(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
            _url = _configuration.GetValue<string>("ApiUrls:Rolls");
            _client = new RestClient(_url);
        }

        public async Task<string> DepartmentDeleteAsync(DepartmentsDeleteVM dataObj)
        {
            var request = new RestRequest("departments/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DepartmentAddAsync(DepartmentsAddEditVM dataObj)
        {
            var request = new RestRequest("departments/add", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DepartmentNameExistsAsync(IsExistsVM dataObj)
        {
            var request = new RestRequest("departments/name/exists", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetDepartmentsAsync()
        {
            var request = new RestRequest($"departments/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetDeletedDepartmentsAsync()
        {
            var request = new RestRequest($"departments/deleted/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DepartmentRestoreAsync(DepartmentsDeleteVM dataObj)
        {
            var request = new RestRequest("departments/restore", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DepartmentPermanentDeleteAsync(DepartmentsDeleteVM dataObj)
        {
            var request = new RestRequest("department/permanent/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }
    }
}
