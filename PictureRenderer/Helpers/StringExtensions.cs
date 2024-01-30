namespace PictureRenderer.Helpers;

public static class StringExtensions
{
    internal static string GetFormatFromExtension(this string filePath)
    {
        var extension = Path.GetExtension(filePath);
        var format = extension.TrimStart('.');
        if (format == "jpeg")
        {
            format = ImageFormat.Jpeg;
        }

        return format;
    }

    internal static Uri GetUriFromPath(this string imagePath)
    {
        if (IsValidHttpUri(imagePath, out var uri))
        {
            return uri;
        }

        //A Uri object must have a domain, but imagePath might be just a path. Add dummy domain, and test again.
        imagePath = "https://dummy-xyz.com" + imagePath;
        if (IsValidHttpUri(imagePath, out uri))
        {
            return uri;
        }

        throw new ArgumentException($"Image url '{imagePath}' is not well formatted.");

    }

    internal static bool IsValidHttpUri(this string uriString, out Uri uri)
    {
        return Uri.TryCreate(uriString, UriKind.Absolute, out uri!) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}