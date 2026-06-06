using Microsoft.EntityFrameworkCore;
using PadelBracket.Components;
using PadelBracket.Data;
using PadelBracket.Repositories;
using PadelBracket.Repositories.Interface;
using PadelBracket.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Data Source=arenapadel.db"));

builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IPlayerAccountRepository, EfPlayerAccountRepository>();
builder.Services.AddSingleton<IPairRepository, InMemoryPairRepository>();
builder.Services.AddSingleton<ITournamentRepository, InMemoryTournamentRepository>();

builder.Services.AddSingleton<TournamentService>();
builder.Services.AddSingleton<StandingService>();
builder.Services.AddSingleton<QualificationService>();
builder.Services.AddSingleton<KnockoutService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddSingleton<PairService>();
builder.Services.AddSingleton<MatchHistoryService>();
builder.Services.AddSingleton<RankingService>();
builder.Services.AddSingleton<TournamentRegistrationService>();
builder.Services.AddScoped<PlayerAccountService>();
builder.Services.AddScoped<IOrganizerRepository, EfOrganizerRepository>();
builder.Services.AddScoped<IOrganizerAccountRepository, EfOrganizerAccountRepository>();
builder.Services.AddScoped<OrganizerService>();
builder.Services.AddScoped<OrganizerAccountService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();