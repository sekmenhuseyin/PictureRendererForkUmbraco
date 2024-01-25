// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace PictureRenderer;

internal static class Picture
{
    internal static string Render(string imagePath, PictureProfileBase profile, LazyLoading lazyLoading)
    {
        return Render(imagePath, profile, string.Empty, lazyLoading);
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, LazyLoading lazyLoading)
    {
        return Render(imagePaths, profile, string.Empty, lazyLoading);
    }

    internal static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
    {
        return Render(imagePath, profile, string.Empty, LazyLoading.Browser, focalPoint);
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, (double x, double y)[] focalPoints)
    {
        return Render(imagePaths, profile, string.Empty, LazyLoading.Browser, focalPoints);
    }

    internal static string Render(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoints)
    {
        return Render(imagePath, profile, altText, LazyLoading.Browser, focalPoints);
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, string altText, (double x, double y)[] focalPoints)
    {
        return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints);
    }

    internal static string Render(string imagePath, PictureProfileBase profile, string altText, string cssClass)
    {
        return Render(imagePath, profile, altText, LazyLoading.Browser, cssClass: cssClass);
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, string altText, string cssClass)
    {
        return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints: default, cssClass: cssClass);
    }

    /// <summary>
    /// Render picture element.
    /// </summary>
    /// <param name="focalPoint">Value range: 0-1 for ImageSharp, 1-[image width/height] for Storyblok.</param>
    /// <returns></returns>
    internal static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default, string cssClass = "")
    {
        var pictureData = profile.GetPictureData(imagePath, altText, focalPoint, cssClass);
           
        var sourceElement = pictureData.RenderSourceElement();

        var sourceElementWebp = string.Empty;
        if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
        {
            sourceElementWebp = pictureData.RenderSourceElement(ImageFormat.Webp);
        }

        var imgElement = profile.RenderImgElement(pictureData, lazyLoading);
        var pictureElement = $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>"; //Webp source element must be rendered first. Browser selects the first version it supports.

        return pictureElement;
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y)[] focalPoints = null, string cssClass = "")
    {
        var pictureData = profile.GetMultiImagePictureData(imagePaths, altText, focalPoints, cssClass);
        var sourceElements = pictureData.RenderSourceElementsForMultiImage();

        var imgElement = profile.RenderImgElement(pictureData, lazyLoading);
        var pictureElement = $"<picture>{sourceElements}{imgElement}</picture>";

        return pictureElement;
    }
}