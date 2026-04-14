using System.Text;
using OpenCvSharp;
using TermCord.Services.Interfaces;

namespace TermCord.Services.Implementation;

public class ImageService : IImageService
{
	private const int MaxWidth = 110;
	private const int MaxHeight = 100;

	private static readonly string[] AsciiRamp =
	[
		"&nbsp;",
		".",
		"-",
		":",
		"+",
		"o",
		"#",
		"▒",
		"▓",
		"█",
	];

	public async Task<string> GetAsciiFromImage(string url)
	{
		using var httpClient = new HttpClient();

		var image = await httpClient.GetByteArrayAsync(url);

		var mat = Cv2.ImDecode(image, ImreadModes.Grayscale);
		
		ResizeToMaxHeight(ref mat);
		ResizeToMaxWidth(ref mat);

		return ConvertToAscii(mat);
	}

	private static void ResizeToMaxWidth(ref Mat input)
	{
		if (input.Width <= MaxWidth)
			return;

		var scale = (double)MaxWidth / input.Width;
		var newHeight = (int)(input.Height * scale);

		Cv2.Resize(input, input, new Size(MaxWidth, newHeight), 0, 0, InterpolationFlags.Lanczos4);
	}

	private static void ResizeToMaxHeight(ref Mat input)
	{
		if (input.Height <= MaxHeight)
			return;

		var scale = (double)MaxHeight / input.Height;
		var newWidth = (int)(input.Width * scale);

		Cv2.Resize(input, input, new Size(newWidth, MaxHeight), 0, 0, InterpolationFlags.Lanczos4);
	}

	private static string ConvertToAscii(Mat mat)
	{
		var sb = new StringBuilder();

		unsafe
		{
			for (var y = 0; y < mat.Height; y++)
			{
				var rowPtr = (byte*)mat.Ptr(y);

				for (var x = 0; x < mat.Width; x++)
				{
					var brightness = rowPtr[x];
					var index = brightness * (AsciiRamp.Length - 1) / 255;
					sb.Append(AsciiRamp[index]);
				}

				sb.Append("<br>");
			}
		}

		return sb.ToString();
	}
}