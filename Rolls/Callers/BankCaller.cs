namespace Rolls.Callers
{
	public class BankCaller : IBankCaller
	{
		private readonly RestClient _client;
		private readonly IConfiguration _configuration;
		private readonly string _url;
		private readonly string _apiKey;

		public BankCaller(IConfiguration configuration)
		{
			_configuration = configuration;
			_apiKey = _configuration.GetValue<string>("ApiKeys:Rolls");
			_url = _configuration.GetValue<string>("ApiUrls:Rolls");
			_client = new RestClient(_url);
		}

		public async Task<string> BankAddAsync(BankAddEditVM dataObj)
		{
			var request = new RestRequest("bank/add", Method.Post);
			request.AddHeader("Api-Key", _apiKey);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(dataObj);
			var result = await _client.ExecuteAsync<string>(request);

			return result.Content;
		}

		public async Task<string> GetBanksAsync()
		{
			var request = new RestRequest($"bank/all", Method.Get);
			request.AddHeader("Api-Key", _apiKey);
			var result = await _client.ExecuteAsync<string>(request);

			return result.Content;
		}

		public async Task<string> BankNameExistsAsync(IsExistsVM dataObj)
		{
			var request = new RestRequest("bank/name/exists", Method.Post);
			request.AddHeader("Api-Key", _apiKey);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(dataObj);
			var result = await _client.ExecuteAsync<string>(request);

			return result.Content;
		}

		public async Task<string> BankDeleteAsync(IdVM dataObj)
		{
			var request = new RestRequest("bank/delete", Method.Post);
			request.AddHeader("Api-Key", _apiKey);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(dataObj);
			var result = await _client.ExecuteAsync<string>(request);

			return result.Content;
		}

		public async Task<string> GetDeletedBanksAsync()
		{
			var request = new RestRequest($"bank/deleted/all", Method.Get);
			request.AddHeader("Api-Key", _apiKey);
			var result = await _client.ExecuteAsync<string>(request);

			return result.Content;
		}

        public async Task<string> BankRestoreAsync(IdVM dataObj)
        {
            var request = new RestRequest("bank/restore", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

        public async Task<string> BankPermanentDeleteAsync(IdVM dataObj)
        {
            var request = new RestRequest("bank/permanent/delete", Method.Post);
            request.AddHeader("Api-Key", _apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(dataObj);
            var result = await _client.ExecuteAsync<string>(request);

            return result.Content;
        }

    }
}
