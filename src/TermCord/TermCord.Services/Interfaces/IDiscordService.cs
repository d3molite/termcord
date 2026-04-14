using Discord;
using Discord.WebSocket;
using TermCord.Services.Models;

namespace TermCord.Services.Interfaces;

public interface IDiscordService
{
	public List<Channel> Channels { get; set; }
	
	public SocketTextChannel? SelectedChannel { get; set; }
	
	public List<IMessage> CurrentChannelMessages { get; set; }

	public string GuildName { get; }

	public event EventHandler? ChannelDisplayUpdate;

	public event EventHandler? ChannelStateUpdate;

	public Task StartAsync(string? token, string? guildId);

	public Task Refresh();
	
	public Task ChangeChannel(ulong channelId);

	public string? GetUsername(ulong userId);
	
	public string? GetRoleName(ulong roleId);

	public List<Channel> ChannelsInCategory(ulong categoryId);
}