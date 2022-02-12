using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Content.Client.UI;

public abstract class StylesheetBase
{
    public abstract Stylesheet Stylesheet { get; }
    protected StyleRule[] BaseRules { get; }

    protected StylesheetBase(IResourceCache resCache)
    {
        var notoSans12 = resCache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);

        BaseRules = new[]
        {
            // Default font.
            new StyleRule(
                new SelectorElement(null, null, null, null),
                new[]
                {
                    new StyleProperty("font", notoSans12),
                }),
        };
    }
}