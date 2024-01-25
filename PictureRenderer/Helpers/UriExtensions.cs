namespace PictureRenderer.Helpers;

public static class UriExtensions
{
    internal static string BuildImageUrl(this Uri uri, PictureProfileBase profile, int imageWidth, int imageHeight, string? wantedFormat, (double x, double y) focalPoint)
    {
        if (profile is not ImageSharpProfile)
        {
            return string.Empty;
        }

        var queryItems = HttpUtility.ParseQueryString(uri.Query);

        if (!string.IsNullOrEmpty(wantedFormat))
        {
            queryItems.Remove("format"); //remove if it already exists
            queryItems.Add("format", wantedFormat);
        }

        queryItems.Add("width", imageWidth.ToString());

        if (queryItems["height"] == null) //add height if it's not already in the querystring.
        {
            var height = GetImageHeight(imageWidth, imageHeight, profile);
            if (height > 0)
            {
                queryItems.Add("height", height.ToString());
            }
        }

        queryItems = AddFocalPointQuery(focalPoint, queryItems);

        queryItems = profile.AddQualityQuery(queryItems);

        return uri.GetImageDomain() + uri.AbsolutePath + "?" + queryItems;
    }

    internal static string BuildSrcSet(this Uri imageUrl, PictureProfileBase profile, string wantedFormat, (double x, double y) focalPoint)
    {
        var srcSetBuilder = new StringBuilder();
        foreach (var width in profile.SrcSetWidths!)
        {
            srcSetBuilder.Append(BuildImageUrl(imageUrl, profile, width, 0, wantedFormat, focalPoint) + " " + width + "w, ");
        }

        return srcSetBuilder.ToString().TrimEnd(',', ' ');
    }

    internal static string GetImageDomain(this Uri uri)
    {
        var domain = string.Empty;
        if (!uri.Host.Contains("dummy-xyz.com"))
        {
            //return the original image url domain.
            domain = uri.GetLeftPart(UriPartial.Authority);
        }

        return domain;
    }

    internal static bool ShouldRenderWebp(this Uri imageUri, PictureProfileBase profile)
    {
        if (profile is not ImageSharpProfile imageSharpProfile)
        {
            return false;
        }

        var originalFormat = imageUri.AbsolutePath.GetFormatFromExtension();
        return imageSharpProfile.CreateWebpForFormat != null && imageSharpProfile.CreateWebpForFormat.Contains(originalFormat);
    }

    internal static int GetImageHeight(int imageWidth, int imageHeight, PictureProfileBase profile)
    {
        //Add height based on aspect ratio, or from FixedHeight.
        if (imageHeight > 0)
        {
            return imageHeight;
        }

        if (profile.AspectRatio > 0)
        {
            return Convert.ToInt32(imageWidth / profile.AspectRatio);
        }

        if (profile.FixedHeight is > 0)
        {
            return profile.FixedHeight.Value;
        }

        return 0;
    }

    internal static NameValueCollection AddFocalPointQuery((double x, double y) focalPoint, NameValueCollection queryItems)
    {
        if ((focalPoint.x > 0 || focalPoint.y > 0) && queryItems["rxy"] == null)
        {
            var (x, y) = FocalPointAsString(focalPoint);
            queryItems.Add("rxy", $"{x},{y}");
        }

        return queryItems;
    }

    internal static (string x, string y) FocalPointAsString((double x, double y) focalPoint)
    {
        var x = Math.Round(focalPoint.x, 3).ToString(CultureInfo.InvariantCulture);
        var y = Math.Round(focalPoint.y, 3).ToString(CultureInfo.InvariantCulture);

        return (x, y);
    }
}