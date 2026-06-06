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
builder.Services.AddScoped<IOrganizerRepository, EfOrganizerRepository>();
builder.Services.AddScoped<IOrganizerAccountRepository, EfOrganizerAccountRepository>();
builder.Services.AddScoped<ITournamentRepository, EfTournamentRepository>();
builder.Services.AddScoped<IPairRepository, EfPairRepository>();
builder.Services.AddScoped<IKnockoutBracketRepository, EfKnockoutBracketRepository>();

builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<PlayerAccountService>();
builder.Services.AddScoped<OrganizerService>();
builder.Services.AddScoped<OrganizerAccountService>();
builder.Services.AddScoped<PairService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<MatchHistoryService>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<KnockoutService>();

builder.Services.AddSingleton<StandingService>();
builder.Services.AddSingleton<QualificationService>();
builder.Services.AddSingleton<TournamentRegistrationService>();

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