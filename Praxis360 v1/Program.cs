using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Components;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DocumentService>();
// Register demo data service and situation service
builder.Services.AddSingleton<DemoSituationAssuranceVieDataService>();
builder.Services.AddSingleton<SituationAssuranceVieService>();
// Register BRIO import services
builder.Services.AddSingleton<IBrioFileReader, BrioCsvFileReader>();
builder.Services.AddSingleton<IBrioImportAnalyzer, BrioImportAnalyzer>();
builder.Services.AddSingleton<IBrioContractApplicationService, BrioContractApplicationService>();
builder.Services.AddSingleton<IClientSelectionService, ClientSelectionService>();
// Register repositories
builder.Services.AddSingleton<IClientRepository, Praxis360_v1.Infrastructure.Repositories.InMemoryClientRepository>();
builder.Services.AddSingleton<IContractRepository, Praxis360_v1.Infrastructure.Repositories.InMemoryContractRepository>();
var app = builder.Build();

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
