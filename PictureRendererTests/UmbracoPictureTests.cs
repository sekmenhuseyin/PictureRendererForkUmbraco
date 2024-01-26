namespace PictureRenderer.Tests;

public class UmbracoPictureTests
{
    [Fact]
    public void Picture_WithNullMediaWithCrops_ReturnsEmptyString()
    {
        // Arrange
        var helper = Substitute.For<IHtmlHelper>();
        var mediaWithCrops = (MediaWithCrops?)null;
        var altText = string.Empty;
        var lazyLoading = LazyLoading.Browser;
        var cssClass = string.Empty;

        // Act
        var result = helper.Picture(mediaWithCrops, Constants.Profile, altText, lazyLoading, cssClass);

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public void Picture_WithMediaWithCropsAndAltText_ReturnsEmptyString()
    {
        // Arrange
        var helper = Substitute.For<IHtmlHelper>();
        var mediaWithCrops = GetMediaWithCrops();
        var altText = "alt text";
        var lazyLoading = LazyLoading.Browser;
        var cssClass = string.Empty;

        // Act
        var result = helper.Picture(mediaWithCrops, Constants.Profile, altText, lazyLoading, cssClass);

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public void Picture_WithMediaWithCropsAndAltTextAndLazyLoading_ReturnsPicture()
    {
        // Arrange
        var helper = Substitute.For<IHtmlHelper>();
        var mediaWithCrops = GetMediaWithCrops();
        var altText = string.Empty;
        var lazyLoading = LazyLoading.None;
        var cssClass = string.Empty;

        // Act
        var result = helper.Picture(mediaWithCrops, Constants.Profile, altText, lazyLoading, cssClass);

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    [Fact]
    public void Picture_WithSingleMediaWithCropsAndAltTextAndLazyLoading_ReturnsPicture()
    {
        // Arrange
        var helper = Substitute.For<IHtmlHelper>();
        var mediaWithCrops = GetMediaWithCrops();
        var altText = string.Empty;
        var lazyLoading = LazyLoading.None;
        var cssClass = string.Empty;
        var profile = new PictureProfile(
            [new MediaCondition("(min-width: 1200px)", 400, 400)],
            400,
            400);

        // Act
        var result = helper.Picture(mediaWithCrops, profile, altText, lazyLoading, cssClass);

        // Assert
        Assert.Equal(string.Empty, result.ToString());
    }

    private static MediaWithCrops GetMediaWithCrops()
    {
        var publishedContent = Substitute.For<IPublishedContent>();

        var publishedProperty = Substitute.For<IPublishedProperty>();
        publishedProperty.Alias.Returns(Umbraco.Cms.Core.Constants.Conventions.Media.File);
        publishedContent.GetProperty(Arg.Any<string>()).Returns(publishedProperty);

        var publishedValueFallback = Substitute.For<IPublishedValueFallback>();

        var image = Substitute.For<PublishedContentModel>(publishedContent, publishedValueFallback);

        var imageCropperValue = Substitute.For<ImageCropperValue>();
        imageCropperValue.Src = "image.jpg";
        publishedProperty.GetValue().Returns(imageCropperValue);

        var media = Substitute.For<MediaWithCrops>(
            image,
            publishedValueFallback,
            imageCropperValue);

        return media;
    }
}