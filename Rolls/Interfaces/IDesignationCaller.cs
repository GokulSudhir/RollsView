namespace Rolls.Interfaces
{
    public interface IDesignationCaller
    {
        public Task<string> GetDesignationsAsync();
        public Task<string> DesignationAddAsync(DesignationAddEditVM dataObj);
        public Task<string> DesignationNameExistsAsync(IsExistsVM dataObj);
        public Task<string> DesignationDeleteAsync(DesignationDeleteVM dataObj);
        public Task<string> GetDeletedDesignationsAsync();
        public Task<string> DesignationRestoreAsync(DesignationDeleteVM dataObj);
        public Task<string> DesignationPermanentDeleteAsync(DesignationDeleteVM dataObj);
        public Task<string> DesignationDropDownAsync();
    }
}
