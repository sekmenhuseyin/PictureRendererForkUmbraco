// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable once CheckNamespace
namespace PictureRenderer;

internal class MediaImagesPictureData : PictureData
{
    public IEnumerable<MediaImagePaths> MediaImages { get; set; }
}