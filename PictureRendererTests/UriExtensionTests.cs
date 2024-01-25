using System.Web;

namespace PictureRenderer.Tests;

public class UriExtensionTests
{
    [Fact]
    public void BuildImageUrl_Returns_Image_Url_With_Width()
    {
        // Arrange
        var uri = new Uri("https://dummy-xyz.com/image.jpg");
        var profile = new ImageSharpProfile();
        var imageWidth = 100;
        var imageHeight = 0;
        var wantedFormat = string.Empty;
        var focalPoint = (0.0, 0.0);

        // Act
        var result = uri.BuildImageUrl(profile, imageWidth, imageHeight, wantedFormat, focalPoint);

        // Assert
        Assert.Equal("/image.jpg?width=100&quality=80", result);
    }

    [Fact]
    public void BuildImageUrl_Returns_Image_Url_With_Width_And_Height()
    {
        // Arrange
        var uri = new Uri("https://dummy-xyz.com/image.jpg");
        var profile = new ImageSharpProfile();
        var imageWidth = 100;
        var imageHeight = 200;
        var wantedFormat = string.Empty;
        var focalPoint = (0.0, 0.0);

        // Act
        var result = uri.BuildImageUrl(profile, imageWidth, imageHeight, wantedFormat, focalPoint);

        // Assert
        Assert.Equal("/image.jpg?width=100&height=200&quality=80", result);
    }

    [Fact]
    public void BuildImageUrl_Returns_Image_Url_With_Width_And_Height_And_Format()
    {
        // Arrange
        var uri = new Uri("https://dummy-xyz.com/image.jpg");
        var profile = new ImageSharpProfile();
        var imageWidth = 100;
        var imageHeight = 200;
        var wantedFormat = "webp";
        var focalPoint = (0.0, 0.0);

        // Act
        var result = uri.BuildImageUrl(profile, imageWidth, imageHeight, wantedFormat, focalPoint);

        // Assert
        Assert.Equal("/image.jpg?format=webp&width=100&height=200&quality=80", result);
    }

    [Fact]
    public void BuildImageUrl_Returns_Image_Url_With_Width_And_Height_And_Format_And_FocalPoint()
    {
        // Arrange
        var uri = new Uri("https://dummy-xyz.com/image.jpg");
        var profile = new ImageSharpProfile();
        var imageWidth = 100;
        var imageHeight = 200;
        var wantedFormat = "webp";
        var focalPoint = (0.5, 0.5);

        // Act
        var result = uri.BuildImageUrl(profile, imageWidth, imageHeight, wantedFormat, focalPoint);

        // Assert
        Assert.Equal("/image.jpg?format=webp&width=100&height=200&rxy=0.5%2c0.5&quality=80", result);
    }
    [Fact]
    public void GetImageDomain_Returns_Image_Domain()
    {
        // Arrange
        var uri = new Uri("https://dummy-xyz.com/image.jpg");

        // Act
        var result = uri.GetImageDomain();

        // Assert
        Assert.Equal("", result);
    }
    [Fact]
    public void GetImageDomain_Returns_Empty_String()
    {
        // Arrange
        var uri = new Uri("https://microsoft.com/image.jpg");

        // Act
        var result = uri.GetImageDomain();

        // Assert
        Assert.Equal("https://microsoft.com", result);
    }

    [Fact]
    public void FocalPointAsString_Returns_FocalPoint_As_String()
    {
        // Arrange
        var focalPoint = (0.5, 0.5);

        // Act
        var result = UriExtensions.FocalPointAsString(focalPoint);

        // Assert
        Assert.Equal(("0.5", "0.5"), result);
    }

    [Fact]
    public void AddFocalPointQuery_Returns_QueryItems_With_FocalPoint()
    {
        // Arrange
        var focalPoint = (0.5, 0.5);
        var queryItems = HttpUtility.ParseQueryString(string.Empty);

        // Act
        var result = UriExtensions.AddFocalPointQuery(focalPoint, queryItems);

        // Assert
        Assert.Equal("0.5,0.5", result["rxy"]);
    }

    [Fact]
    public void AddFocalPointQuery_Returns_QueryItems_Without_FocalPoint()
    {
        // Arrange
        var focalPoint = (0.0, 0.0);
        var queryItems = HttpUtility.ParseQueryString("rxy=0.5,0.5");

        // Act
        var result = UriExtensions.AddFocalPointQuery(focalPoint, queryItems);

        // Assert
        Assert.Equal("0.5,0.5", result["rxy"]);
    }

    [Fact]
    public void GetImageHeight_Returns_Image_Height()
    {
        // Arrange
        var imageWidth = 100;
        var imageHeight = 200;
        var profile = new ImageSharpProfile();

        // Act
        var result = UriExtensions.GetImageHeight(imageWidth, imageHeight, profile);

        // Assert
        Assert.Equal(200, result);
    }

    [Fact]
    public void GetImageHeight_Returns_Image_Height_With_Aspect_Ratio()
    {
        // Arrange
        var imageWidth = 100;
        var imageHeight = 0;
        var profile = new ImageSharpProfile
        {
            AspectRatio = 1
        };

        // Act
        var result = UriExtensions.GetImageHeight(imageWidth, imageHeight, profile);

        // Assert
        Assert.Equal(100, result);
    }

    [Fact]
    public void GetImageHeight_Returns_Image_Height_With_FixedHeight()
    {
        // Arrange
        var imageWidth = 100;
        var imageHeight = 0;
        var profile = new ImageSharpProfile
        {
            FixedHeight = 57
        };

        // Act
        var result = UriExtensions.GetImageHeight(imageWidth, imageHeight, profile);

        // Assert
        Assert.Equal(57, result);
    }
}