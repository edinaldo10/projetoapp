using CloudApplication.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a Controllers com Views
builder.Services.AddControllersWithViews();

// Configuração limpa de Health Checks (sem duplicidade e sem warning de BuildServiceProvider precoce)
builder.Services.AddHealthChecks()
    .AddCheck("Database", () =>
    {
        try
        {
            // Usa o container de serviços após o build para verificar o banco de forma segura
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return db.Database.CanConnect()
                ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is online")
                : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database is offline");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }, tags: new[] { "ready" });

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = databaseUrl
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=orders;Username=postgres;Password=postgres";

if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
        var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var port = uri.Port > 0 ? uri.Port : 5432;

        connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={user};Password={pass};SSL Mode=Prefer;Trust Server Certificate=true";
    }
    catch
    {
        connectionString = databaseUrl;
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase) ||
        connectionString.Contains("Filename=", StringComparison.OrdinalIgnoreCase) ||
        connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Garantir criação do banco ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Mapeamento dos Health Checks para Cloud-Native / DevOps
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Apenas valida se o processo da aplicação está rodando no ar
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready") // Valida se a aplicação e o banco de dados estão conectados
});

// Rota padrão do MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

// Documentação Scalar
app.MapScalarApiReference(options =>
{
    options.Title = "API de Pedidos (.NET)";
});

app.Run();

public partial class Program { }