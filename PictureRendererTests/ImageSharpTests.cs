namespace PictureRenderer.Tests;

public class ImageSharpTests
{
    [Fact]
    public void RenderSingleImageTest()
    {
        const string expected = "<picture>" +
            "<source srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80 400w, " +
                            "/myImage.jpg?format=webp&width=200&height=200&quality=80 200w, " +
                            "/myImage.jpg?format=webp&width=100&height=100&quality=80 100w\" " +
                    "sizes=\"(min-width: 1200px), (min-width: 600px), (min-width: 300px)\" " +
                    "type=\"image/webp\"/>" +
            "<source srcset=\"/myImage.jpg?width=400&height=400&quality=80 400w, " +
                            "/myImage.jpg?width=200&height=200&quality=80 200w, " +
                            "/myImage.jpg?width=100&height=100&quality=80 100w\" " +
                    "sizes=\"(min-width: 1200px), (min-width: 600px), (min-width: 300px)\" />" +
            "<img src=\"/myImage.jpg?width=400&height=400&quality=80\" alt=\"\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var result = Picture.Render("/myImage.jpg", GetTestImageProfile());

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMultiImageTest()
    {
        const string expected = "<picture>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80, /myImage.jpg?format=webp&width=800&height=800&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&quality=80, /myImage.jpg?width=800&height=800&quality=80 2x\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&quality=80, /myImage2.png?width=400&height=400&quality=80 2x\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&quality=80, /myImage3.jpg?format=webp&width=200&height=200&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&quality=80, /myImage3.jpg?width=200&height=200&quality=80 2x\"/>" +
            "<img src=\"/myImage.jpg?width=400&height=400&quality=80\" alt=\"\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var result = Picture.Render(["/myImage.jpg", "/myImage2.png", "/myImage3.jpg"], GetTestImageProfile());

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMultiImageWithWidthAndHeightTest()
    {
        const string expected = "<picture>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80, /myImage.jpg?format=webp&width=800&height=800&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&quality=80, /myImage.jpg?width=800&height=800&quality=80 2x\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?format=webp&width=200&height=200&quality=80, /myImage2.png?format=webp&width=400&height=400&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&quality=80, /myImage2.png?width=400&height=400&quality=80 2x\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.gif?width=100&height=100&quality=80, /myImage3.gif?width=200&height=200&quality=80 2x\"/>" +

            "<img src=\"/myImage.jpg?width=400&height=400&quality=80\" alt=\"\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var profile = Constants.Profile with
        {
            FetchPriority = FetchPriority.None
        };
        var result = Picture.Render(["/myImage.jpg", "/myImage2.png", "/myImage3.gif"], profile);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMultiImageMissingImageTest()
    {
        const string expected = "<picture>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80, /myImage.jpg?format=webp&width=800&height=800&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&quality=80, /myImage.jpg?width=800&height=800&quality=80 2x\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.jpg?format=webp&width=200&height=200&quality=80, /myImage2.jpg?format=webp&width=400&height=400&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.jpg?width=200&height=200&quality=80, /myImage2.jpg?width=400&height=400&quality=80 2x\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage2.jpg?format=webp&width=100&height=100&quality=80, /myImage2.jpg?format=webp&width=200&height=200&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage2.jpg?width=100&height=100&quality=80, /myImage2.jpg?width=200&height=200&quality=80 2x\"/>" +
            "<img src=\"/myImage.jpg?width=400&height=400&quality=80\" alt=\"alt text\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var result = Picture.Render(["/myImage.jpg", "/myImage2.jpg"], GetTestImageProfile(), "alt text");

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMultiImageWithFocalPointsTest()
    {
        const string expected = "<picture>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&rxy=0.1%2c0.1&quality=80, /myImage.jpg?format=webp&width=800&height=800&rxy=0.1%2c0.1&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80, /myImage.jpg?width=800&height=800&rxy=0.1%2c0.1&quality=80 2x\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&rxy=0.2%2c0.2&quality=80, /myImage2.png?width=400&height=400&rxy=0.2%2c0.2&quality=80 2x\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&rxy=0.3%2c0.3&quality=80, /myImage3.jpg?format=webp&width=200&height=200&rxy=0.3%2c0.3&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&rxy=0.3%2c0.3&quality=80, /myImage3.jpg?width=200&height=200&rxy=0.3%2c0.3&quality=80 2x\"/>" +
            "<img src=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\" alt=\"\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var result = Picture.Render(["/myImage.jpg", "/myImage2.png", "/myImage3.jpg"], GetTestImageProfile(), new[] { (0.1, 0.1), (0.2, 0.2), (0.3, 0.3) });

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMultiImageWithEmptyFocalPointsTest()
    {
        const string expected = "<picture>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&rxy=0.1%2c0.1&quality=80, /myImage.jpg?format=webp&width=800&height=800&rxy=0.1%2c0.1&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80, /myImage.jpg?width=800&height=800&rxy=0.1%2c0.1&quality=80 2x\"/>" +
            "<source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&quality=80, /myImage2.png?width=400&height=400&quality=80 2x\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&rxy=0.3%2c0.3&quality=80, /myImage3.jpg?format=webp&width=200&height=200&rxy=0.3%2c0.3&quality=80 2x\" type=\"image/webp\"/>" +
            "<source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&rxy=0.3%2c0.3&quality=80, /myImage3.jpg?width=200&height=200&rxy=0.3%2c0.3&quality=80 2x\"/>" +
            "<img src=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\" alt=\"\" width=\"400\" height=\"400\" loading=\"lazy\" decoding=\"async\" />" +
        "</picture>";
        var result = Picture.Render(["/myImage.jpg", "/myImage2.png", "/myImage3.jpg"], GetTestImageProfile(), new[] { (0.1, 0.1), default, (0.3, 0.3) });

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RenderMethodTests()
    {
        var result = Picture.Render("/myImage.jpg", GetTestImageProfile(), LazyLoading.Browser);
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render(["/myImage.jpg", "/myImage2.jpg"], GetTestImageProfile(), LazyLoading.Browser);
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render("/myImage.jpg", GetTestImageProfile(), (1,5));
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render(["/myImage.jpg", "/myImage2.jpg"], GetTestImageProfile(), [(1,5), (1,5)]);
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render("/myImage.jpg", GetTestImageProfile(), "alt text", (1,5));
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render(["/myImage.jpg", "/myImage2.jpg"], GetTestImageProfile(), "alt text", [(1,5), (1,5)]);
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render("/myImage.jpg", GetTestImageProfile(), "alt text", "css");
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);

        result = Picture.Render(["/myImage.jpg", "/myImage2.jpg"], GetTestImageProfile(), "alt text", "css");
        Assert.NotEmpty(result);
        Assert.NotNull(result);
        Assert.Contains("<picture>", result);
        Assert.Contains("</picture>", result);
        Assert.Contains("<source", result);
        Assert.Contains("<img", result);
    }

    private static PictureProfile GetTestImageProfile()
    {
        return Constants.Profile with
        {
            CreateWebpForFormat = [ImageFormat.Jpeg],
            FetchPriority = FetchPriority.None
        };
    }
}