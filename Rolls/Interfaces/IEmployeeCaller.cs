namespace Rolls.Interfaces
{
    public interface IEmployeeCaller
    {
        public Task<string> EmployeeAddAsync(EmployeeAddEditVM dataObj);
        public Task<string> EmployeeExistsAsync(DoesEmployeeExist dataObj);
        public Task<string> GetEmployeesAsync();
        public Task<string> EmployeeDeleteAsync(EmployeeDeleteVM dataObj);
        public Task<string> GetDeletedEmployeesAsync();
        public Task<string> EmployeeRestoreAsync(EmployeeDeleteVM dataObj);
        public Task<string> EmployeePermanentDeleteAsync(EmployeeDeleteVM dataObj);
    }
}
