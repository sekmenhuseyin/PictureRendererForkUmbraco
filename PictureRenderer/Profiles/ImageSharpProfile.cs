namespace PictureRenderer.Profiles;

// ReSharper disable once ClassNeverInstantiated.Global
public class ImageSharpProfile : PictureProfileBase
{
    public ImageSharpProfile()
    {
        Quality = 80;
        CreateWebpForFormat = [ImageFormat.Jpeg, ImageFormat.Png];
    }

    /// <summary>
    /// The image formats that should be offered as webp versions.
    /// PictureRenderer.ImageFormat.Jpeg is added by default.
    /// </summary>
    public string[] CreateWebpForFormat { get; set; }
}