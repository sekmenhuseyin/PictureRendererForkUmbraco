#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable once CheckNamespace
namespace PictureRenderer;

public class MediaImagesPictureData : PictureData
{
    public IEnumerable<MediaImagePaths> MediaImages { get; set; }
}

public class MediaImagePaths
{
    public string ImagePath { get; set; }
    public string ImagePathWebp { get; set; }
    public string MediaCondition { get; set; }
}