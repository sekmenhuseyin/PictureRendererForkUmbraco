using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PictureRenderer.Profiles;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
// ReSharper disable UnusedParameter.Local

namespace PictureRenderer;

public static class UmbracoPicture
{
    public static HtmlString Picture(this IHtmlHelper helper, MediaWithCrops? mediaWithCrops, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
    {
        if (mediaWithCrops == null)
        {
            return new HtmlString(string.Empty);
        }

        if (string.IsNullOrEmpty(altText) && !string.IsNullOrEmpty((string?)mediaWithCrops.Content.GetProperty("pictureAltText")?.GetValue()))
        {
            altText = (string)mediaWithCrops.Content.GetProperty("pictureAltText")?.GetValue()!;
        }

        if (mediaWithCrops.LocalCrops.HasFocalPoint())
        {
            return Picture(helper, mediaWithCrops.LocalCrops, profile, altText, lazyLoading);
        }

        return Picture(helper, (ImageCropperValue?)mediaWithCrops.Content.GetProperty("umbracofile")?.GetValue(), profile, altText, lazyLoading, cssClass);
    }

    /// <summary>
    /// the middle man that forwards all parameters to picture renderer
    /// </summary>
    private static HtmlString Picture(this IHtmlHelper helper, ImageCropperValue? imageCropper, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
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

        if (profile.MultiImageMediaConditions?.Length == null)
        {
            return new HtmlString(PictureRenderer.Picture.Render(imageCropper.Src!, profile, altText, lazyLoading, focalPoint, cssClass));
        }

        var imageSources = profile.MultiImageMediaConditions!.Select(_ => imageCropper.Src!).ToArray();
        var focalPoints = profile.MultiImageMediaConditions!.Select(_ => focalPoint).ToArray();
        return new HtmlString(PictureRenderer.Picture.Render(imageSources, profile, altText, lazyLoading, focalPoints, cssClass));
    }
}