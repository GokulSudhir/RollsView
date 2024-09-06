using System.Security.Policy;

namespace Rolls.Callers
{
    public class EmployeeCaller : IEmployeeCaller
    {
        private readonly RestClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly string _apiKey;

        public EmployeeCaller(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
            _url = _configuration.GetValue<string>("ApiUrls:Rolls");
            _client = new RestClient(_url);
        }

        public async Task<string> EmployeeAddAsync(EmployeeAddEditVM dataObj)
        {
            var request = new RestRequest("employee/add", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> EmployeeExistsAsync(DoesEmployeeExist dataObj)
        {
            var request = new RestRequest("employee/exists", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetEmployeesAsync()
        {
            var request = new RestRequest($"employee/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> EmployeeDeleteAsync(EmployeeDeleteVM dataObj)
        {
            var request = new RestRequest("employee/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetDeletedEmployeesAsync()
        {
            var request = new RestRequest($"employee/deleted/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> EmployeeRestoreAsync(EmployeeDeleteVM dataObj)
        {
            var request = new RestRequest("employee/restore", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> EmployeePermanentDeleteAsync(EmployeeDeleteVM dataObj)
        {
            var request = new RestRequest("employee/permanent/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }
    }
}
