namespace PictureRenderer;
// ReSharper disable UnusedParameter.Local

public static class UmbracoPicture
{
    public static HtmlString Picture(this IHtmlHelper helper, MediaWithCrops mediaWithCrops, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
    {
        if (mediaWithCrops == null)
        {
            return new HtmlString(string.Empty);
        }

        if (string.IsNullOrEmpty(altText) && !string.IsNullOrEmpty((string)mediaWithCrops.Content.GetProperty("pictureAltText")?.GetValue()))
        {
            altText = (string)mediaWithCrops.Content.GetProperty("pictureAltText")?.GetValue()!;
        }

        if (mediaWithCrops.LocalCrops.HasFocalPoint())
        {
            return Picture(helper, mediaWithCrops.LocalCrops, profile, altText, lazyLoading);
        }

        return Picture(helper, (ImageCropperValue)mediaWithCrops.Content.GetProperty(Constants.Conventions.Media.File)?.GetValue(), profile, altText, lazyLoading, cssClass);
    }

    /// <summary>
    /// the middle man that forwards all parameters to picture renderer
    /// </summary>
    private static HtmlString Picture(this IHtmlHelper helper, ImageCropperValue imageCropper, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
    {
        if (imageCropper == null)
        {
            return new HtmlString(string.Empty);
        }

        (double x, double y) focalPoint = default;
        if (imageCropper.HasFocalPoint())
        {
            focalPoint.x = decimal.ToDouble(imageCropper.FocalPoint!.Left);
            focalPoint.y = decimal.ToDouble(imageCropper.FocalPoint.Top);
        }

        var imageSources = profile.MultiImageMediaConditions.Select(_ => imageCropper.Src!).ToArray();
        var focalPoints = profile.MultiImageMediaConditions.Select(_ => focalPoint).ToArray();

        var pictureData = PictureRenderer.Picture.Render(imageSources, profile, altText, lazyLoading, focalPoints, cssClass);
        return new HtmlString(pictureData);
    }
}