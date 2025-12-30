using DotnetBadWordDetector;

namespace T.Helpers;

public static class ProfanityHelper
{
	private static readonly ProfanityDetector Detector = new(allLocales: true);

	public static string Filter(string? content)
	{
		return Detector.MaskProfanity(content);
	}
}
