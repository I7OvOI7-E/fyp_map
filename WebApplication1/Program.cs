
using Microsoft.EntityFrameworkCore; 
using WebApplication1;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddScoped<ServiceMap>();
builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var connectionString = builder.Configuration.GetConnectionString("MapConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Set ConnectionStrings__MapConnection to your Neon Npgsql connection string.");

builder.Services.AddDbContext<MyDbContextMap>(options =>
    options.UseNpgsql(connectionString));
var app = builder.Build();

// Initialize this project's empty Neon database from the EF model.
// This does not update existing tables; use migrations for future schema changes.
await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<MyDbContextMap>();
    var created = await database.Database.EnsureCreatedAsync();
    app.Logger.LogInformation(created
        ? "Database initialized: Devices, IncidentReports, IncidentImages, Charts."
        : "Database already initialized; existing tables left unchanged.");
}

app.UseStaticFiles();


app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
