namespace PictureRenderer.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void GetFormatFromExtension_ReturnsJpeg_WhenExtensionIsJpeg()
    {
        var filePath = "image.jpeg";
        var format = filePath.GetFormatFromExtension();
        Assert.Equal(ImageFormat.Jpeg, format);

        filePath = "image.jpg";
        format = filePath.GetFormatFromExtension();
        Assert.Equal(ImageFormat.Jpeg, format);
    }

    [Fact]
    public void GetUriFromPath_ReturnsUri_WhenPathIsValid()
    {
        var imagePath = "https://dummy-xyz.com/image.jpeg";
        var uri = imagePath.GetUriFromPath();
        Assert.Equal(imagePath, uri.ToString());

        imagePath = "/image.jpeg";
        uri = imagePath.GetUriFromPath();
        Assert.Equal("https://dummy-xyz.com/image.jpeg", uri.ToString());

        imagePath = "\\image.jpeg";
        Assert.Throws<ArgumentException>(() => imagePath.GetUriFromPath());
    }

    [Fact]
    public void IsValidHttpUri_ReturnsTrue_WhenUriIsValid()
    {
        var uriString = "https://dummy-xyz.com/image.jpeg";
        var isValid = uriString.IsValidHttpUri(out var uri);
        Assert.True(isValid);
        Assert.Equal(uriString, uri.ToString());

        uriString = "http://dummy-xyz.com/image.jpeg";
        isValid = uriString.IsValidHttpUri(out uri);
        Assert.True(isValid);
        Assert.Equal(uriString, uri.ToString());

        uriString = "ftp://dummy-xyz.com/image.jpeg";
        isValid = uriString.IsValidHttpUri(out uri);
        Assert.False(isValid);
    }
}