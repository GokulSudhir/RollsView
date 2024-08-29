namespace Rolls.Interfaces
{
	public interface IBankCaller
	{
		public Task<string> GetBanksAsync();
		public Task<string> BankAddAsync(BankAddEditVM dataObj);
		public Task<string> BankNameExistsAsync(IsExistsVM dataObj);
		public Task<string> BankDeleteAsync(IdVM dataObj);
		public Task<string> GetDeletedBanksAsync();
        public Task<string> BankRestoreAsync(IdVM dataObj);
        public Task<string> BankPermanentDeleteAsync(IdVM dataObj);

    }
}
