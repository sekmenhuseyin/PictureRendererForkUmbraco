// ReSharper disable once CheckNamespace
namespace PictureRenderer;

internal record MediaImagesPictureData : PictureData
{
    public IEnumerable<MediaImagePaths> MediaImages { get; init; }
}