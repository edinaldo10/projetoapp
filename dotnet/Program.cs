using CloudApplication.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// 1. Registro de Health Check mais limpo
builder.Services.AddHealthChecks()
    .AddCheck("Database", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });

// Configuração do DB (Mantida como você tinha, mas garantindo que o WebApplicationFactory possa sobrescrevê-la)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = databaseUrl ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Port=5432;Database=orders;Username=postgres;Password=postgres";

// Lógica de string de conexão (Mantida igual para não quebrar produção)
if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Prefer;Trust Server Certificate=true";
    }
    catch { connectionString = databaseUrl; }
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString.Contains("Data Source=") || connectionString.Contains(":memory:"))
        options.UseSqlite(connectionString);
    else
        options.UseNpgsql(connectionString);
});

var app = builder.Build();

// --- Ajuste na lógica de Middleware para não quebrar em testes ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

// 2. HealthCheck corrigido para usar a injeção de dependência corretamente
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        // Usamos o requestServices que já está no contexto do middleware, 
        // sendo compatível tanto com app real quanto com o Factory de teste
        var db = context.RequestServices.GetService<AppDbContext>();
        if (db != null && await db.Database.CanConnectAsync())
        {
            await context.Response.WriteAsync("{\"status\":\"Healthy\"}");
        }
        else
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync("{\"status\":\"Unhealthy\"}");
        }
    }
});

app.MapControllerRoute(name: "default", pattern: "{controller=Orders}/{action=Index}/{id?}");

app.MapScalarApiReference();

app.Run();

public partial class Program { }