﻿namespace PictureRenderer.Helpers;

public static class UriExtensions
{
    internal static string BuildImageUrl(this Uri uri, PictureProfile profile, int imageWidth, int imageHeight, string wantedFormat, (double x, double y) focalPoint)
    {
        var queryItems = HttpUtility.ParseQueryString(uri.Query);

        if (!string.IsNullOrEmpty(wantedFormat))
        {
            queryItems.Remove("format"); //remove if it already exists
            queryItems.Add("format", wantedFormat);
        }

        queryItems.Add("width", imageWidth.ToString());
        queryItems.Add("height", imageHeight.ToString());
        queryItems = AddFocalPointQuery(focalPoint, queryItems);
        queryItems = profile.AddQualityQuery(queryItems);

        return uri.GetImageDomain() + uri.AbsolutePath + "?" + queryItems;
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

    internal static bool ShouldRenderWebp(this Uri imageUri, PictureProfile profile)
    {
        var originalFormat = imageUri.AbsolutePath.GetFormatFromExtension();
        return profile.CreateWebpForFormat != null && profile.CreateWebpForFormat.Contains(originalFormat);
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