namespace PictureRenderer.Profiles;

public abstract class PictureProfileBase
{
    private int _fallBackWidth;
    private int _fallBackHeight;

    public int[] SrcSetWidths { get; set; }
    public string[] Sizes { get; set; }

    /// <summary>
    /// Use this when you want to show different images depending on media condition (for example different image for mobile sized screen and desktop sized screen).
    /// </summary>
    public MediaCondition[] MultiImageMediaConditions { get; set; }

    /// <summary>
    /// Default value is 80.
    /// </summary>
    public int? Quality { get; set; }


    /// <summary>
    /// Image width for browsers without support for picture element. Will use the largest image if not set.
    /// </summary>
    public int ImageWidth
    {
        get
        {
            return _fallBackWidth switch
            {
                0 when MultiImageMediaConditions != null => MultiImageMediaConditions.MaxBy(mcw => mcw.Width)?.Width ?? default,
                0 when SrcSetWidths != null => SrcSetWidths.Max(),
                _ => _fallBackWidth
            };
        }
        set => _fallBackWidth = value;
    }

    /// <summary>
    ///     Image height for browsers without support for picture element. Will use the largest image if not set.
    /// </summary>
    public int ImageHeight
    {
        get { return _fallBackHeight == default && MultiImageMediaConditions != null
            ? MultiImageMediaConditions.MaxBy(mcw => mcw.Height)?.Height ?? default
            : _fallBackHeight;
        }
        set => _fallBackHeight = value;
    }

    /// <summary>
    /// The wanted aspect ratio of the image (width/height).
    /// Example: An image with aspect ratio 16:9 = 16/9 = 1.777.
    /// </summary>
    public double AspectRatio { get; set; }

    /// <summary>
    /// Set a fixed height for all image sizes. Overrides the aspect ratio setting.  
    /// </summary>
    public int? FixedHeight { get; set; }

    /// <summary>
    /// If true, width and height attributes will be rendered on the img element.
    /// </summary>
    public bool ImgWidthHeight { get; set; } = true;

    /// <summary>
    /// Img element decoding attribute.
    /// </summary>
    public ImageDecoding ImageDecoding {get; set;} = ImageDecoding.Async;

    /// <summary>
    /// Img element fetchPriority attribute.
    /// </summary>
    public FetchPriority FetchPriority {get; set;} = FetchPriority.Low;
}