using System.Collections.Specialized;

namespace PictureRenderer.Tests;

public class PictureExtensionsTests
{
    [Fact]
    public void GetImgWidthAndHeightAttributesShouldWork()
    {
        var profile = Constants.Profile;
        var imageDetails = profile.GetImgWidthAndHeightAttributes();
        Assert.Equal("width=\"400\" height=\"400\" ", imageDetails);
    }

    [Fact]
    public void AddQualityQueryShouldWork()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query.AddQualityQuery(80);
        Assert.Equal("quality=80", query.ToString());

        query.AddQualityQuery(80);
        Assert.Equal("quality=80", query.ToString());
    }
}