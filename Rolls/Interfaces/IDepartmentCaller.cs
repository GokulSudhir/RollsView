namespace Rolls.Interfaces
{
  public interface IDepartmentCaller
  {
    public Task<string> GetDepartmentsAsync();
    public Task<string> DepartmentAddAsync(DepartmentAddEditVM dataObj);
    public Task<string> DepartmentNameExistsAsync(IsExistsVM dataObj);
  }
}
