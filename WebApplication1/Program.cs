
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;
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

builder.Services.AddDbContext<MyDbContextMap>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MapConnection")));
var app = builder.Build();


app.UseStaticFiles();


app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
