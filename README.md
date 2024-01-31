# ASP.Net Picture Renderer
The Picture renderer makes it easy to optimize images in (pixel) size, quality, file size, and image format. 
Images will be responsive, and can be lazy loaded.
It's a light-weight library, suitable for Umbraco.

The Picture Renderer renders an [HTML picture element](https://webdesign.tutsplus.com/tutorials/quick-tip-how-to-use-html5-picture-for-responsive-images--cms-21015). The picture element presents a set of images in different sizes and formats. 
It’s then up to the browser to select the most appropriate image depending on screen resolution, viewport width, network speed, and the rules that you set up.

If you are unfamiliar with the details of the Picture element, then I highly recommend reading
 [this](https://webdesign.tutsplus.com/tutorials/quick-tip-how-to-use-html5-picture-for-responsive-images--cms-21015) and/or [this](https://www.smashingmagazine.com/2014/05/responsive-images-done-right-guide-picture-srcset/).

This fork of Picture Renderer only works together with Umbraco

## Why should you use this?
You want the images on your web site to be as optimized as possible. For example, having the most optimal image for any screen size and device type, 
will make the web page load faster, 
and is a [Google search rank factor](https://developers.google.com/search/docs/advanced/guidelines/google-images#optimize-for-speed).
The content editor doesn't have to care about what aspect ratio, or size, the image has. The most optimal image will always be used.<br>

### Webp format
The rendered picture element will also contain [webp](https://developers.google.com/speed/webp/) versions of the image. 
By default this will be rendered for jpg and png images.<br>

## How to use
* Add the [PictureRenderer](https://www.nuget.org/packages/PictureRendererForkUmbraco/) nuget.
* Create an ImageSharpProfile for the different types of images that you have on your web site. A Picture profile describes how an image should be scaled in various cases. <br>
You could for example create Picture profiles for: “Top hero image”, “Teaser image”, “Image gallery thumbnail”.
* Let Picture Renderer create the picture HTML element.

### Picture profile

#### Examples
```c#
using PictureRenderer.Profiles;

public static class PictureProfiles
{
    public static readonly ImageSharpProfile BannerImage = new([
        new MediaCondition("(max-width: 474.95px)", Width: 475, Height: 304),
        new MediaCondition("(min-width: 475px) and (max-width: 767.95px)", Width: 768, Height: 440),
        new MediaCondition("(min-width: 768px) and (max-width: 1279.95px)", Width: 1440, Height: 501),
        new MediaCondition("(min-width: 1280px) and (max-width: 1439.95px)", Width: 1440, Height: 501),
        new MediaCondition("(min-width: 1440px)", Width: 1920, Height: 501)
    ], ImageWidth: 1440, ImageHeight: 501);
}
```

* **MultiImageMediaConditions** - Define image widths for different media conditions. 
* **ImageWidth** - Define image width for image tag. 
* **ImageHeight** - Define image height for image tag. 
* **ImageDecoding (optional)** - Value for img element `decoding` attribute. Default value: `async`.
* **FetchPriority (optional)** - Value for img element `fetchPriority` attribute. Default value: `none` (unset)

### Render picture element
Render the picture element by calling `@Html.Picture`
<br>
#### Parameters
* **Media** - MediaWithCrops.
* **profile** - The Picture profile that specifies image widths, etc..
* **altText (optional)** - Img element `alt` attribute.
* **lazyLoading (optional)** - Type of lazy loading. Currently only [browser native lazy loading](https://developer.mozilla.org/en-US/docs/Web/Performance/Lazy_loading#images_and_iframes) (or none).
* **focalPoint/focalPoints (optional)** - Use focal point when image is cropped (multiple points for multiple image paths). 
* **cssClass (optional)** - Css class for img element. 

Picture.Render returns a string, so you need to make sure the string is not HTML-escaped by using Html.Raw or similar.
<br> 

Basic MVC/Razor page sample
```
@Html.Picture(Model.MediaWithCrops, Model.Profile, Model.AltText, Model.LazyLoading, Model.ImageClass)
```
<br><br>

### How to see that it actually works
You can see that different images are selected for different devices and screen sizes. Note that the Chrome (Chromium based) browser will not select a smaller image if a larger one is already downloaded. It may be easier to see the actual behaviour when using e.g. Firefox.
