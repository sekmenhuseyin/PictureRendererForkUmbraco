#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable once CheckNamespace
namespace PictureRenderer;

public class PictureData
{
    public string SrcSet { get; set; }

    public string SrcSetWebp { get; set; }

    public string SizesAttribute { get; set; }

    public string ImgSrc { get; set; }

    public string AltText { get; set; }
    public string CssClass { get; set; }
}