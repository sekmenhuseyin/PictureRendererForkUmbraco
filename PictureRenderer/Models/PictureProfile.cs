// ReSharper disable once CheckNamespace
namespace PictureRenderer;

/// <summary>
/// Create a simple picture profile to render responsive images.
/// </summary>
/// <param name="MultiImageMediaConditions">Use this to show different images depending on media condition (for example different image for mobile sized screen and desktop sized screen)</param>
/// <param name="ImageWidth">Image width for browsers without support for picture element</param>
/// <param name="ImageHeight">Image height for browsers without support for picture element</param>
/// <param name="FetchPriority">Img element fetchPriority attribute</param>
/// <param name="ImageDecoding">Img element decoding attribute</param>
public record ImageSharpProfile(
    MediaCondition[] MultiImageMediaConditions,
    int ImageWidth,
    int ImageHeight,
    FetchPriority FetchPriority = FetchPriority.Low,
    ImageDecoding ImageDecoding = ImageDecoding.Async
)
{
    /// <summary>
    /// The image formats that should be offered as webp versions.
    /// PictureRenderer.ImageFormat.Jpeg is added by default.
    /// </summary>
    public string[] CreateWebpForFormat { get; init; } = [ImageFormat.Jpeg, ImageFormat.Png];

    /// <summary>
    /// Default value is 80.
    /// </summary>
    public int Quality { get; init; } = 80;
}