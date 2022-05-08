using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using static Robust.Client.UserInterface.StylesheetHelpers;
using GameEditorView = Content.Client.GameView.GameEditorView;

namespace Content.Client.UI;

public abstract class StylesheetBase
{
    public const string BaseButton = "ButtonBase";
    
    protected StyleBoxFlat BackgroundOuterTexture { get; }
    protected StyleBoxFlat BackgroundInnerTexture { get; }
    protected StyleBoxFlat BaseButtonTexture { get; }
    
    public abstract Stylesheet Stylesheet { get; }
    protected StyleRule[] BaseRules { get; }

    protected StylesheetBase(IResourceCache resCache)
    {
        var notoSans12 = resCache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
        var buttonTex = resCache.GetTexture("/Textures/Interface/button.png");
        
        BackgroundOuterTexture = new StyleBoxFlat
        {
            BackgroundColor = new Color(0x21, 0x21, 0x26),
        };
        
        BackgroundInnerTexture = new StyleBoxFlat
        {
            BackgroundColor = new Color(0x1b, 0x1b, 0x1f),
        };
        
        BaseButtonTexture = new StyleBoxFlat
        {
            BackgroundColor = new Color(0x27, 0x27, 0x2e),
            ContentMarginLeftOverride = 4,
            ContentMarginTopOverride = 4,
            ContentMarginBottomOverride = 4,
            ContentMarginRightOverride = 4,
        };
        
        var flatWhite = new StyleBoxFlat
        {
            BackgroundColor = Color.White,
            ContentMarginLeftOverride = 10,
            ContentMarginTopOverride = 10
        };

        BaseRules = new[]
        {
            // Default font.
            new StyleRule(
                new SelectorElement(null, null, null, null),
                new[]
                {
                    new StyleProperty("font", notoSans12),
                }),
            new StyleRule(
                new SelectorElement(typeof(Button), null, null, null),
                new[]
                {
                    new StyleProperty(ContainerButton.StylePropertyStyleBox, BaseButtonTexture),
                }),
            Element<MenuBar>().Prop(PanelContainer.StylePropertyPanel, BackgroundOuterTexture),
            Element<GameEditorView>().Prop(PanelContainer.StylePropertyPanel, BackgroundInnerTexture),
            Element<PanelContainer>().Class("Background").Prop(PanelContainer.StylePropertyPanel, BackgroundInnerTexture),
            Element<Slider>().Prop(Slider.StylePropertyGrabber, flatWhite),
            Element<Slider>().Prop(Slider.StylePropertyFill, flatWhite),
        };
    }
}