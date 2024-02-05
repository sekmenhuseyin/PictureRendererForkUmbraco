namespace PictureRenderer.Helpers;

public static class PictureExtensions
{
    internal static PictureData GetPictureData(this ImageSharpProfile profile, string imagePath, string altText, (double x, double y) focalPoint, string cssClass)
    {
        var uri = imagePath.GetUriFromPath();
        var minWidth = profile.MultiImageMediaConditions.MinBy(m => m.Width).Width;
        var minHeight = profile.MultiImageMediaConditions.MinBy(m => m.Height).Height;
        var lqipProfile = profile with { Quality = 20 };

        var pData = new PictureData
        {
            AltText = altText,
            ImgSrc = uri.BuildImageUrl(profile, profile.ImageWidth, profile.ImageHeight, string.Empty, focalPoint),
            ImgLqipSrc = uri.BuildImageUrl(lqipProfile, minWidth, minHeight, string.Empty, focalPoint),
            CssClass = cssClass,
            SrcSet = uri.BuildSrcSet(profile, string.Empty, focalPoint),
            SizesAttribute = string.Join(", ", profile.MultiImageMediaConditions.Select(m => m.MediaQuery)),
            SrcSetWebp = uri.BuildSrcSet(profile, ImageFormat.Webp, focalPoint)
        };

        return pData;
    }

    internal static MediaImagesPictureData GetMultiImagePictureData(this ImageSharpProfile profile, string[] imagePaths, string altText, (double x, double y)[] focalPoints, string cssClass)
    {
        Uri fallbackImageUri = default!;
        (double x, double y) fallbackImageFocalPoint = default;
        var numberOfImages = imagePaths.Length;
        var numberOfFocalPoints = focalPoints.Length;
        var mediaImagePaths = new List<MediaImagePaths>();
        for (var i = 0; i < profile.MultiImageMediaConditions.Length; i++)
        {
            //If there isn't images for all media conditions, use last image in list.
            var imageIndex = i >= numberOfImages ? numberOfImages - 1 : i;
            var imagePath = imagePaths[imageIndex];
            //Get focal point for this image (if there is one)
            var imageFocalPoint = imageIndex >= numberOfFocalPoints ? default : focalPoints[imageIndex];

            var imageUri = imagePath.GetUriFromPath();
            mediaImagePaths.Add(new MediaImagePaths
            {
                ImagePath = imageUri.BuildImageUrl(profile, profile.MultiImageMediaConditions[i].Width, profile.MultiImageMediaConditions[i].Height, null, imageFocalPoint, true),
                ImagePathWebp = imageUri.ShouldRenderWebp(profile) ?
                    imageUri.BuildImageUrl(profile, profile.MultiImageMediaConditions[i].Width, profile.MultiImageMediaConditions[i].Height, ImageFormat.Webp, imageFocalPoint, true)
                    : string.Empty,
                MediaCondition = profile.MultiImageMediaConditions[i].MediaQuery
            });

            if (i == 0)
            {
                //use first image as fallback image
                fallbackImageUri = imageUri;
                fallbackImageFocalPoint = imageFocalPoint;
            }
        }

        var minWidth = profile.MultiImageMediaConditions.MinBy(m => m.Width).Width;
        var minHeight = profile.MultiImageMediaConditions.MinBy(m => m.Height).Height;
        var lqipProfile = profile with { Quality = 20 };
        var pData = new MediaImagesPictureData
        {
            MediaImages = mediaImagePaths,
            AltText = altText,
            ImgSrc = fallbackImageUri.BuildImageUrl(profile, profile.ImageWidth, profile.ImageHeight, string.Empty, fallbackImageFocalPoint),
            ImgLqipSrc = fallbackImageUri.BuildImageUrl(lqipProfile, minWidth, minHeight, string.Empty, fallbackImageFocalPoint),
            CssClass = cssClass
        };

        return pData;
    }

    internal static string RenderImgElement(this ImageSharpProfile profile, PictureData pictureData, LazyLoading lazyLoading)
    {
        var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile);
        var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
        var classAttribute = string.IsNullOrEmpty(pictureData.CssClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.CssClass)}\"";
        var decodingAttribute = profile.ImageDecoding == ImageDecoding.None ? string.Empty : $"decoding=\"{Enum.GetName(typeof(ImageDecoding), profile.ImageDecoding)?.ToLower()}\" ";
        var fetchPriorityAttribute = profile.FetchPriority == FetchPriority.None ? string.Empty : $"fetchPriority=\"{Enum.GetName(typeof(FetchPriority), profile.FetchPriority)?.ToLower()}\" ";
        var imgLqipSrcAttribute = string.Empty;

        if (lazyLoading is LazyLoading.Browser)
        {
            imgLqipSrcAttribute = $"src=\"{pictureData.ImgLqipSrc}\" data-";
        }

        return $"<img {imgLqipSrcAttribute}src=\"{pictureData.ImgSrc}\" alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{fetchPriorityAttribute}{classAttribute}/>";
    }

    internal static void AddQualityQuery(this NameValueCollection queryItems, int quality)
    {
        if (queryItems["quality"] == null)
        {
            if (quality > 0)
            {
                //Add quality value from profile.
                queryItems.Add("quality", quality.ToString());
            }
        }
        else
        {
            //Quality value already exists in querystring. Don't change it, but make sure it's last (after format).
            var qualityValue = queryItems["quality"];
            queryItems.Remove("quality");
            queryItems.Add("quality", qualityValue);
        }
    }

    internal static string GetImgWidthAndHeightAttributes(this ImageSharpProfile profile)
    {
        var widthAttribute = $"width=\"{profile.ImageWidth}\" ";
        var heightAttribute = $"height=\"{profile.ImageHeight}\" ";

        return widthAttribute + heightAttribute;

    }

    internal static string RenderSourceElement(this PictureData pictureData, LazyLoading lazyLoading, string format = "")
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

        if (lazyLoading == LazyLoading.Browser)
        {
            srcSetAttribute = $"data-src=\"{srcSet}\" data-" + srcSetAttribute;
        }

        return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute}/>";
    }

    internal static string RenderSourceElementsForMultiImage(this MediaImagesPictureData pictureData, LazyLoading lazyLoading)
    {
        var sourceElementsBuilder = new StringBuilder();
        foreach (var mediaImage in pictureData.MediaImages)
        {
            var mediaAttribute = $"media=\"{mediaImage.MediaCondition}\"";

            //add webp source element first
            if (!string.IsNullOrEmpty(mediaImage.ImagePathWebp))
            {
                var srcSetWebpAttribute = $"srcset=\"{mediaImage.ImagePathWebp}\"";

                if (lazyLoading == LazyLoading.Browser)
                {
                    srcSetWebpAttribute = $"data-src=\"{mediaImage.ImagePathWebp}\" data-" + srcSetWebpAttribute;
                }

                var formatAttribute = "type=\"image/webp\"";
                sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetWebpAttribute} {formatAttribute}/>");
            }

            var srcSetAttribute = $"srcset=\"{mediaImage.ImagePath}\"";

            if (lazyLoading == LazyLoading.Browser)
            {
                srcSetAttribute = $"data-src=\"{mediaImage.ImagePath}\" data-" + srcSetAttribute;
            }

            sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetAttribute}/>");
        }

        return sourceElementsBuilder.ToString();
    }
}