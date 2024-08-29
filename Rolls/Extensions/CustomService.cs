namespace Rolls.Extensions
{
    public static class CustomService
    {
        public static IServiceCollection CustomServices(this IServiceCollection services)
        {
            services.AddSingleton<ICountryCaller, CountryCaller>();
            services.AddSingleton<IBankCaller, BankCaller>();
            services.AddSingleton<IDepartmentCaller, DepartmentCaller>();

            return services;
        }
    }
}
