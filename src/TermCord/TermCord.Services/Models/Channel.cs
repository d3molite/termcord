using Discord;

namespace TermCord.Services.Models;

public class Channel
{
	public ulong Id { get; set; }
	
	public string Name { get; set; }

	public ChannelType ChannelType { get; set; }
	
	public IChannel Object { get; set; }
	
	public bool HasUnreadMessages { get; set; }
}