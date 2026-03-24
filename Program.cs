using InclusiveCode.API.Data;
using InclusiveCode.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext using SQLite (local database)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=inclusive_code.db"));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
}

// Services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PythonAnalyzerService>();

builder.Services.AddHttpClient<PythonAnalyzerService>(client =>
{
    var baseUrl = builder.Configuration["PythonAnalyzer:DevUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HTTPS redirection port for local development
// builder.Services.AddHttpsRedirection(options => ...);

var app = builder.Build();


// ?? Apply migrations safely
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao aplicar migrations:");
        Console.WriteLine(ex.Message);
    }
}


// ?? Test connection safely
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        var canConnect = dbContext.Database.CanConnect();
        Console.WriteLine(canConnect
            ? "Conectado ao banco com sucesso!"
            : "Falha ao conectar ao banco!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao testar conexão:");
        Console.WriteLine(ex.Message);
    }
}


// Middlewares
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

// Only use HTTPS redirection when not in Development to avoid redirect warnings
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();