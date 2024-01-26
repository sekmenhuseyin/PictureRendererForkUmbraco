namespace PictureRenderer.Helpers;

public static class PictureExtensions
{
    internal static PictureData GetPictureData(this PictureProfile profile, string imagePath, string altText, (double x, double y) focalPoint, string cssClass)
    {
        if (profile.SrcSetWidths == null || profile.Sizes == null)
        {
            throw new ArgumentException("SrcSetWidths and/or Sizes are not defined in Picture profile.");
        }

        var uri = imagePath.GetUriFromPath();

        var pData = new PictureData
        {
            AltText = altText,
            ImgSrc = uri.BuildImageUrl(profile, profile.ImageWidth, profile.ImageHeight, string.Empty, focalPoint),
            CssClass = cssClass,
            SrcSet = uri.BuildSrcSet(profile, string.Empty, focalPoint),
            SizesAttribute = string.Join(", ", profile.Sizes)
        };

        if (uri.ShouldRenderWebp(profile))
        {
            pData.SrcSetWebp = uri.BuildSrcSet(profile, ImageFormat.Webp, focalPoint);
        }

        return pData;
    }

    internal static MediaImagesPictureData GetMultiImagePictureData(this PictureProfile profile, string[] imagePaths, string altText, (double x, double y)[]? focalPoints, string cssClass)
    {
        if (profile.MultiImageMediaConditions == null || profile.MultiImageMediaConditions.Length == 0)
        {
            throw new ArgumentException("MultiImageMediaConditions must be defined in Picture profile when rendering multiple images.");
        }

        focalPoints ??= Array.Empty<(double x, double y)>();

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
                ImagePath = imageUri.BuildImageUrl(profile, profile.MultiImageMediaConditions[i].Width, profile.MultiImageMediaConditions[i].Height, null, imageFocalPoint),
                ImagePathWebp = imageUri.ShouldRenderWebp(profile) ? imageUri.BuildImageUrl(profile, profile.MultiImageMediaConditions[i].Width, profile.MultiImageMediaConditions[i].Height, ImageFormat.Webp, imageFocalPoint) : string.Empty,
                MediaCondition = profile.MultiImageMediaConditions[i].Media
            });

            if (i == 0)
            {
                //use first image as fallback image
                fallbackImageUri = imageUri;
                fallbackImageFocalPoint = imageFocalPoint;
            }
        }

        var pData = new MediaImagesPictureData
        {
            MediaImages = mediaImagePaths,
            AltText = altText,
            ImgSrc = fallbackImageUri.BuildImageUrl(profile, profile.ImageWidth, profile.ImageHeight, string.Empty, fallbackImageFocalPoint),
            CssClass = cssClass
        };

        return pData;
    }

    internal static string RenderImgElement(this PictureProfile profile, PictureData pictureData, LazyLoading lazyLoading)
    {
        var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile);
        var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
        var classAttribute = string.IsNullOrEmpty(pictureData.CssClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.CssClass)}\"";
        var decodingAttribute = profile.ImageDecoding == ImageDecoding.None ? string.Empty : $"decoding=\"{Enum.GetName(typeof(ImageDecoding), profile.ImageDecoding)?.ToLower()}\" ";
        var fetchPriorityAttribute = profile.FetchPriority == FetchPriority.None ? string.Empty : $"fetchPriority=\"{Enum.GetName(typeof(FetchPriority), profile.FetchPriority)?.ToLower()}\" ";

        return $"<img src=\"{pictureData.ImgSrc}\" alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{fetchPriorityAttribute}{classAttribute}/>";
    }

    internal static NameValueCollection AddQualityQuery(this PictureProfile profile, NameValueCollection queryItems)
    {
        if (queryItems["quality"] == null)
        {
            if (profile.Quality != null)
            {
                //Add quality value from profile.
                queryItems.Add("quality", profile.Quality.ToString());
            }
        }
        else
        {
            //Quality value already exists in querystring. Don't change it, but make sure it's last (after format).
            var quality = queryItems["quality"];
            queryItems.Remove("quality");
            queryItems.Add("quality", quality);
        }

        return queryItems;
    }

    internal static string GetImgWidthAndHeightAttributes(this PictureProfile profile)
    {
        if (!profile.ImgWidthHeight)
        {
            return string.Empty;
        }

        var widthAttribute = $"width=\"{profile.ImageWidth}\" ";
        var heightAttribute = "";
        if (profile.ImageHeight > 0)
        {
            heightAttribute = $"height=\"{profile.ImageHeight}\" ";
        }
        else if (profile.AspectRatio > 0)
        {
            heightAttribute = $"height=\"{Math.Round(profile.ImageWidth / profile.AspectRatio)}\" ";
        }
        else if (profile.FixedHeight is > 0)
        {
            heightAttribute = $"height=\"{profile.FixedHeight}\" ";
        }

        return widthAttribute + heightAttribute;

    }

    internal static string RenderSourceElement(this PictureData pictureData, string format = "")
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

    internal static string RenderSourceElementsForMultiImage(this MediaImagesPictureData pictureData)
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