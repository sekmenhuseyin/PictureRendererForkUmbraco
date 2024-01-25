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
    internal static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default, string cssClass = "", string imgWidth = "", string style = "")
    {
        var pictureData = profile.GetPictureData(imagePath, altText, focalPoint, cssClass);
           
        var sourceElement = RenderSourceElement(pictureData);

        var sourceElementWebp = string.Empty;
        if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
        {
            sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
        }
            
        var imgElement = RenderImgElement(pictureData, profile, lazyLoading, imgWidth, style);
        var pictureElement = $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>"; //Webp source element must be rendered first. Browser selects the first version it supports.

        return pictureElement;
    }

    /// <summary>
    /// Render different images in the same picture element.
    /// </summary>
    internal static string Render(string[] imagePaths, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y)[] focalPoints = null, string cssClass = "")
    {
        var pictureData = profile.GetMultiImagePictureData(imagePaths, altText, focalPoints, cssClass);
        var sourceElements = RenderSourceElementsForMultiImage(pictureData);
        var imgElement = RenderImgElement(pictureData, profile, lazyLoading, string.Empty, string.Empty);
        var pictureElement = $"<picture>{sourceElements}{imgElement}</picture>";

        return pictureElement;
    }

    internal static string RenderImgElement(PictureData pictureData, PictureProfileBase profile, LazyLoading lazyLoading, string imgWidth, string style)
    {
        var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile, imgWidth);
        var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
        var classAttribute = string.IsNullOrEmpty(pictureData.CssClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.CssClass)}\"";
        var decodingAttribute = profile.ImageDecoding == ImageDecoding.None ? string.Empty :  $"decoding=\"{Enum.GetName(typeof(ImageDecoding), profile.ImageDecoding)?.ToLower()}\" ";
        var fetchPriorityAttribute = profile.FetchPriority == FetchPriority.None ? string.Empty :  $"fetchPriority=\"{Enum.GetName(typeof(FetchPriority), profile.FetchPriority)?.ToLower()}\" ";
        var styleAttribute = string.IsNullOrEmpty(style) ? string.Empty : $"style=\"{style}\" ";

        return $"<img alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" src=\"{pictureData.ImgSrc}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{fetchPriorityAttribute}{classAttribute}{styleAttribute}/>";
    }

    internal static string GetImgWidthAndHeightAttributes(PictureProfileBase profile, string imgWidth)
    {
        if (!string.IsNullOrEmpty(imgWidth))
        {
            return $"width=\"{imgWidth}\" ";
        }
        if(profile.ImgWidthHeight)
        {
            var widthAttribute = $"width=\"{profile.ImageWidth}\" ";
            var heightAttribute = "";
            if (profile.AspectRatio > 0)
            {
                heightAttribute = $"height=\"{Math.Round(profile.ImageWidth / profile.AspectRatio)}\" ";
            } 
            else if (profile.FixedHeight != null && profile.FixedHeight > 0)
            {
                heightAttribute = $"height=\"{profile.FixedHeight}\" ";

            }
            return widthAttribute + heightAttribute ;
        }

        return string.Empty;
    }

    internal static string RenderSourceElement(PictureData pictureData, string format = "")
    {
        var srcSet = pictureData.SrcSet;
        var formatAttribute = string.Empty;
        if (format == ImageFormat.Webp)
        {
            srcSet = pictureData.SrcSetWebp;
            formatAttribute = "type=\"image/" + format + "\"";
        }
        var srcSetAttribute = $"srcset=\"{srcSet}\"";
        var sizesAttribute = $"sizes=\"{pictureData.SizesAttribute}\"";

        return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute}/>";
    }

    internal static string RenderSourceElementsForMultiImage(MediaImagesPictureData pictureData)
    {
        var sourceElementsBuilder = new StringBuilder();
        foreach (var mediaImage in pictureData.MediaImages)
        {
            var mediaAttribute = $"media=\"{mediaImage.MediaCondition}\"";

            //add webp source element first
            if (!string.IsNullOrEmpty(mediaImage.ImagePathWebp))
            {
                var srcSetWebpAttribute = $"srcset=\"{mediaImage.ImagePathWebp}\"";
                var formatAttribute = "type=\"image/webp\"";
                sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetWebpAttribute} {formatAttribute}/>");
            }

            var srcSetAttribute = $"srcset=\"{mediaImage.ImagePath}\"";
            sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetAttribute}/>");
        }

        return sourceElementsBuilder.ToString();
    }
}