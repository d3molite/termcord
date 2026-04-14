using Discord;
using Discord.WebSocket;
using TermCord.Services.Interfaces;
using TermCord.Services.Models;

namespace TermCord.Services.Implementation;

public partial class DiscordService : IDiscordService
{
	private DiscordSocketClient _client;
	private SocketGuild? _guild;
	private ulong _guildId;

	public List<Channel> Channels { get; set; } = [];

	public SocketTextChannel? SelectedChannel { get; set; }

	public List<IMessage> CurrentChannelMessages { get; set; } = [];

	public string GuildName => _guild?.Name ?? "NOT LOGGED IN";

	public event EventHandler? ChannelDisplayUpdate;

	public event EventHandler? ChannelStateUpdate;

	public DiscordService()
	{
		_client = new DiscordSocketClient(
			new DiscordSocketConfig()
			{
				GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
				MessageCacheSize = 300,
				AlwaysDownloadUsers = true,
			}
		);

		_client.Ready += OnClientReady;
		_client.MessageReceived += OnMessageReceived;
		_client.MessageUpdated += OnMessageEdited;
		_client.MessageDeleted += OnMessageDeleted;
	}

	public async Task StartAsync(string? token, string? guildId)
	{
        ArgumentNullException.ThrowIfNull(token);
		ArgumentNullException.ThrowIfNull(guildId);

		if (!ulong.TryParse(guildId, out _guildId))
			throw new ArgumentException("Guild ID is not a valid guild ID");

        await _client.LoginAsync(
			TokenType.Bot,
			token
		);

		await _client.StartAsync();

		await Task.Delay(-1);
	}

	public async Task Refresh()
	{
		await GetChannels();
	}

	public async Task ChangeChannel(ulong channelId)
	{
		SelectedChannel = _guild?.Channels.FirstOrDefault(x => x.Id == channelId) as SocketTextChannel;
		var messages = SelectedChannel!.GetMessagesAsync(40);

		var list = await messages.SelectMany(x => x)
			.ToListAsync();

		CurrentChannelMessages.Clear();
		ChannelDisplayUpdate?.Invoke(this, EventArgs.Empty);

		CurrentChannelMessages = list.OrderBy(x => x.Timestamp)
			.ToList();

		Channels.First(x => x.Id == SelectedChannel.Id)
			.HasUnreadMessages = false;

		ChannelDisplayUpdate?.Invoke(this, EventArgs.Empty);
		ChannelStateUpdate?.Invoke(this, EventArgs.Empty);
	}

	public string? GetUsername(ulong userId)
	{
		var user = _guild?.GetUser(userId);
		return user?.Username;
	}

	public string? GetRoleName(ulong roleId)
	{
		return _guild?.Roles.FirstOrDefault(x => x.Id == roleId)
			?.Name;
	}

	private async Task OnClientReady()
	{
		_guild = _client.GetGuild(_guildId);
		await Refresh();
	}

	private async Task GetChannels()
	{
		if (_guild is null)
			return;

		Channels = _guild.Channels.Where(ChannelDisplayed)
			.OrderBy(x => x.Position)
			.Select(x => new Channel()
				{
					Id = x.Id,
					Name = x.Name,
					ChannelType = x.ChannelType,
					Object = x,
				}
			)
			.ToList();

		ChannelDisplayUpdate?.Invoke(this, EventArgs.Empty);
	}

	public List<Channel> ChannelsInCategory(ulong categoryId)
	{
		return Channels.Where(x => x.Object is ITextChannel tc && tc.CategoryId == categoryId)
			.ToList();
	}

	private static bool ChannelDisplayed(SocketGuildChannel channel)
		=> channel.ChannelType != ChannelType.Forum && channel.ChannelType != ChannelType.PublicThread;

	private bool IsActiveChannel(IChannel channel)
		=> channel.Id == SelectedChannel?.Id;
}