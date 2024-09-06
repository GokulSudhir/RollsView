namespace Rolls.Interfaces
{
    public interface IDepartmentsCaller
    {
        public Task<string> GetDepartmentsAsync();
        public Task<string> DepartmentAddAsync(DepartmentsAddEditVM dataObj);
        public Task<string> DepartmentNameExistsAsync(IsExistsVM dataObj);
        public Task<string> DepartmentDeleteAsync(DepartmentsDeleteVM dataObj);
        public Task<string> GetDeletedDepartmentsAsync();
        public Task<string> DepartmentRestoreAsync(DepartmentsDeleteVM dataObj);
        public Task<string> DepartmentPermanentDeleteAsync(DepartmentsDeleteVM dataObj);
        public Task<string> DepartmentDropDownAsync();
    }
}
