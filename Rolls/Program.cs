try
{

    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration().
                    MinimumLevel.Information().
                    MinimumLevel.Override("Microsoft", LogEventLevel.Warning).
                    MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information).
                    WriteTo.PostgreSQL(
                    connectionString: builder.Configuration.GetConnectionString("EFConn"),
                    tableName: "rolls_logs",
                    columnOptions: null,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    needAutoCreateTable: true).Enrich.WithMachineName().
                    Enrich.FromLogContext().
                    Enrich.WithWebApiControllerName().Enrich.WithWebApiActionName().Enrich.WithHttpRequestType().CreateLogger();

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

    builder.Services.CustomServices();
    builder.Services.AddSignalR();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    //app.MapHub<NotificationHub>("/notificationHub");


    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Admin startup failed due to {reason}", ex.GetBaseException().Message);
}
finally
{
    Log.CloseAndFlush();
}

