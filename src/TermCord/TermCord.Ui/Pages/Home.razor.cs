using TermCord.Services.Models;

namespace TermCord.Ui.Pages;

public partial class Home
{
	private bool IsIgnored(Channel channel)
	{
		return channel.Name is "internal" or "Archiv";
	}
}