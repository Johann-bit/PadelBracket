using PadelBracket.Components;
using PadelBracket.Repositories;
using PadelBracket.Repositories.Interface;
using PadelBracket.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
builder.Services.AddSingleton<IPairRepository, InMemoryPairRepository>();
builder.Services.AddSingleton<ITournamentRepository, InMemoryTournamentRepository>();

builder.Services.AddSingleton<TournamentService>();
builder.Services.AddSingleton<StandingService>();
builder.Services.AddSingleton<QualificationService>();
builder.Services.AddSingleton<KnockoutService>();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<PairService>();
builder.Services.AddSingleton<MatchHistoryService>();
builder.Services.AddSingleton<RankingService>();

var app = builder.Build();

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