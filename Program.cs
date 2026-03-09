using InclusiveCode.API.Data;
using InclusiveCode.API.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DbContext only when a connection string is provided.
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(defaultConnection))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(defaultConnection));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PythonAnalyzerService>();
builder.Services.AddHttpClient<PythonAnalyzerService>(client =>
{
    var baseUrl = builder.Configuration["PythonAnalyzer:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

var app = builder.Build();

// Enable OpenAPI/Swagger in all environments (production included).
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
