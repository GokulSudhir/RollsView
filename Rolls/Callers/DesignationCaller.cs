
using System.Security.Policy;

namespace Rolls.Callers
{
    public class DesignationCaller : IDesignationCaller
    {
        private readonly RestClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly string _apiKey;

        public DesignationCaller(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
            _url = _configuration.GetValue<string>("ApiUrls:Rolls");
            _client = new RestClient(_url);
        }

        public async Task<string> GetDesignationsAsync()
        {
            var request = new RestRequest($"designation/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationAddAsync(DesignationAddEditVM dataObj)
        {
            var request = new RestRequest("designation/add", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationNameExistsAsync(IsExistsVM dataObj)
        {
            var request = new RestRequest("designation/name/exists", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationDeleteAsync(DesignationDeleteVM dataObj)
        {
            var request = new RestRequest("designation/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> GetDeletedDesignationsAsync()
        {
            var request = new RestRequest($"designation/deleted/all", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationRestoreAsync(DesignationDeleteVM dataObj)
        {
            var request = new RestRequest("designation/restore", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationPermanentDeleteAsync(DesignationDeleteVM dataObj)
        {
            var request = new RestRequest("designation/permanent/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> DesignationDropDownAsync()
        {
            var request = new RestRequest($"designation/dropdown", Method.Get);
            request.AddHeader("Api-Key", _apiKey);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }
    }
}
