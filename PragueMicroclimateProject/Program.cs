using PragueMicroclimateProject.DependencyInjection;
using Serilog;
using System.Reflection;

// create bootstrap serilog logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Information("Application is starting");

    // ### Logging ###

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console(); // always write to console (do not have to be in serilog config)
    });

    // ### Add services ###

    builder.Services.AddControllers();

    builder.Services.AddSwaggerGen(options =>
    {
        // add description to the models / endpoints
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddHybridCache();

    // ### Add custom services ###

    builder.Services.AddApplicationOptions();
    builder.Services.AddWebServices();

    // ### Build app ###

    var app = builder.Build();

    app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage(); // detailed error page for development
    }

    // swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSerilogRequestLogging(); // log HTTP requests

    app.UseStatusCodePages();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
