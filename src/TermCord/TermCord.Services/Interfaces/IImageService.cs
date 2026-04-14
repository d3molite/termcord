namespace TermCord.Services.Interfaces;

public interface IImageService
{
	public Task<string> GetAsciiFromImage(string url);
}