using TermCord.Main.Components;
using TermCord.Main.Helpers;
using TermCord.Services.Implementation;
using TermCord.Services.Interfaces;
using TermCord.Ui.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddSingleton<IDiscordService, DiscordService>();
builder.Services.AddSingleton<IImageService, ImageService>();

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
	.AddInteractiveServerRenderMode()
	.AddAdditionalAssemblies(typeof(Home).Assembly);

var loader = new EnvLoader();

var discordService = app.Services.GetRequiredService<IDiscordService>();
Task.Run(async () => await discordService.StartAsync(loader.Get("BOT_TOKEN"), loader.Get("GUILD_ID")));

await app.RunAsync();