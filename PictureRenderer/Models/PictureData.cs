// ReSharper disable once CheckNamespace
namespace PictureRenderer;

internal record PictureData
{
    public string SrcSet { get; init; }

    public string SrcSetWebp { get; init; }

    public string SizesAttribute { get; init; }

    public string ImgSrc { get; init; }
    public string ImgLqipSrc { get; init; }

    public string AltText { get; init; }

    public string CssClass { get; init; }
}