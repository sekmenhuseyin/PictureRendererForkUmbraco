namespace PictureRenderer.Tests;

public static class Constants
{
    public static readonly PictureProfile Profile = new(
        [
            new MediaCondition("(min-width: 1200px)", 400, 400),
            new MediaCondition("(min-width: 600px)", 200, 200),
            new MediaCondition("(min-width: 300px)", 100, 100)
        ],
        400,
        400
    );
}