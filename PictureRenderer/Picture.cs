// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace PictureRenderer;

internal static class Picture
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal static string Render(string imagePath, ImageSharpProfile profile, LazyLoading lazyLoading)
    {
        return Render(imagePath, profile, string.Empty, lazyLoading);
    }

    internal static string Render(string[] imagePaths, ImageSharpProfile profile, LazyLoading lazyLoading)
    {
        return Render(imagePaths, profile, string.Empty, lazyLoading);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal static string Render(string imagePath, ImageSharpProfile profile, (double x, double y) focalPoint)
    {
        return Render(imagePath, profile, string.Empty, LazyLoading.Browser, focalPoint);
    }

    internal static string Render(string[] imagePaths, ImageSharpProfile profile, (double x, double y)[] focalPoints)
    {
        return Render(imagePaths, profile, string.Empty, LazyLoading.Browser, focalPoints);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal static string Render(string imagePath, ImageSharpProfile profile, string altText, (double x, double y) focalPoints)
    {
        return Render(imagePath, profile, altText, LazyLoading.Browser, focalPoints);
    }

    internal static string Render(string[] imagePaths, ImageSharpProfile profile, string altText, (double x, double y)[] focalPoints)
    {
        return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    internal static string Render(string imagePath, ImageSharpProfile profile, string altText, string cssClass)
    {
        return Render(imagePath, profile, altText, LazyLoading.Browser, cssClass: cssClass);
    }

    internal static string Render(string[] imagePaths, ImageSharpProfile profile, string altText, string cssClass)
    {
        return Render(imagePaths, profile, altText, LazyLoading.Browser, default, cssClass);
    }

    /// <summary>
    /// Render picture element.
    /// </summary>
    public static string Render(string imagePath, ImageSharpProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default, string cssClass = "")
    {
        if (lazyLoading == LazyLoading.Browser)
        {
            cssClass += " lazyload";
        }

        var pictureData = profile.GetPictureData(imagePath, altText, focalPoint, cssClass.Trim());

        var sourceElement = pictureData.RenderSourceElement(lazyLoading);

        var sourceElementWebp = string.Empty;
        if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
        {
            sourceElementWebp = pictureData.RenderSourceElement(lazyLoading, ImageFormat.Webp);
        }

        var imgElement = profile.RenderImgElement(pictureData, lazyLoading);
        var pictureElement = $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>"; //Webp source element must be rendered first. Browser selects the first version it supports.

        return pictureElement;
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    public static string Render(string[] imagePaths, ImageSharpProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y)[] focalPoints = null, string cssClass = "")
    {
        if (lazyLoading == LazyLoading.Browser)
        {
            cssClass += " lazyload";
        }

        focalPoints ??= Array.Empty<(double x, double y)>();
        var pictureData = profile.GetMultiImagePictureData(imagePaths, altText, focalPoints, cssClass);
        var sourceElements = pictureData.RenderSourceElementsForMultiImage(lazyLoading);

        var imgElement = profile.RenderImgElement(pictureData, lazyLoading);
        var pictureElement = $"<picture>{sourceElements}{imgElement}</picture>";

        return pictureElement;
    }
}