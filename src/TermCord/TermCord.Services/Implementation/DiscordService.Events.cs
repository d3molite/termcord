using Discord;
using Discord.WebSocket;

namespace TermCord.Services.Implementation;

public partial class DiscordService
{
	private async Task OnMessageEdited(
		Cacheable<IMessage, ulong> _,
		SocketMessage newMessage,
		ISocketMessageChannel channel
	)
	{
		if (!IsActiveChannel(channel))
			return;

		var editedMessageIndex = CurrentChannelMessages.FindIndex(x => x.Id == newMessage.Id);

		if (editedMessageIndex == -1)
			return;

		CurrentChannelMessages[editedMessageIndex] = newMessage;
		ChannelDisplayUpdate?.Invoke(this, EventArgs.Empty);
	}

	private async Task OnMessageReceived(SocketMessage arg)
	{
		if (arg.Channel is not SocketGuildChannel channel)
			return;

		if (!IsActiveChannel(channel))
		{
			var c = Channels.FirstOrDefault(x => x.Id == arg.Channel.Id);
			if (c == null) return;

			c.HasUnreadMessages = true;
			ChannelStateUpdate?.Invoke(this, EventArgs.Empty);
			return;
		}

		CurrentChannelMessages.Add(arg);
		ChannelDisplayUpdate?.Invoke(this, EventArgs.Empty);
	}

	private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		if (!message.HasValue)
			return;

		if (!IsActiveChannel(message.Value.Channel))
			return;

		var editedMessage = CurrentChannelMessages.Find(x => x.Id == message.Id);

		if (editedMessage is null)
			return; 
		
		CurrentChannelMessages.Remove(editedMessage);
	}
}