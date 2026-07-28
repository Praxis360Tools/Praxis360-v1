using Microsoft.EntityFrameworkCore;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Components;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Repositories;
using Praxis360_v1.Infrastructure.Services;
using Praxis360_v1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register database infrastructure
builder.Services.AddSingleton<IDatabasePathResolver, LocalAppDataDatabasePathResolver>();
builder.Services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

builder.Services.AddDbContextFactory<AppDbContext>((sp, options) =>
{
    var pathResolver = sp.GetRequiredService<IDatabasePathResolver>();
    var dbPath = pathResolver.GetDatabasePath();
    var connectionString = $"Data Source={dbPath};Foreign Keys=True";
    options.UseSqlite(connectionString);
});

builder.Services.AddSingleton<DocumentService>();
// Register demo data service and situation service
builder.Services.AddSingleton<DemoSituationAssuranceVieDataService>();
builder.Services.AddSingleton<SituationAssuranceVieService>();
// Register BRIO import services
builder.Services.AddSingleton<IBrioFileReader, BrioCsvFileReader>();
builder.Services.AddSingleton<IBrioImportAnalyzer, BrioImportAnalyzer>();
builder.Services.AddScoped<IBrioContractApplicationService, BrioContractApplicationService>();
builder.Services.AddScoped<IClientSelectionService, ClientSelectionService>();
// Register repositories
builder.Services.AddScoped<IClientRepository, EfCoreClientRepository>();
builder.Services.AddScoped<IContractRepository, EfCoreContractRepository>();
// Register persistence service
builder.Services.AddScoped<IBrioPersistenceService, EfCoreBrioPersistenceService>();

var app = builder.Build();

// Initialize database before starting
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
