namespace Rolls.Interfaces
{
  public interface ICountryCaller
  {
    public Task<string> CountryNameExistsAsync(IsExistsVM dataObj);
    public Task<string> GetCountriesAsync();

  }
}
