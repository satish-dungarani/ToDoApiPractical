using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ToDoApiPractical.DataContext;
using ToDoApiPractical.Helpers;
using ToDoApiPractical.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

// Configure log4net
string logDirectory = Path.Combine(Environment.CurrentDirectory, "ErrorLogs");

// Ensure the log directory exists
if (!Directory.Exists(logDirectory))
    Directory.CreateDirectory(logDirectory);

// Set the global property for log directory
Environment.SetEnvironmentVariable("LOG_DIRECTORY", logDirectory + Path.DirectorySeparatorChar);

// Configure log4net
var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
var configPath = Path.Combine(Environment.CurrentDirectory, "log4net.config");
XmlConfigurator.Configure(logRepository, new FileInfo(configPath));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<TodoItemService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITodoItemService, TodoItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();
 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

LogHelper.logger.Info("Application started!");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

    // Ensure DB is migrated and seeded
    await context.Database.MigrateAsync();
    await AppDbContextFetch.SeedTodoItemsAsync(context, httpClientFactory);
}

app.Run();
