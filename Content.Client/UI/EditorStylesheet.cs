using System.Linq;
using Content.Client.Rendering;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using static Robust.Client.UserInterface.StylesheetHelpers;

namespace Content.Client.UI;

public sealed class EditorStylesheet : StylesheetBase
{
    public EditorStylesheet(IResourceCache resCache) : base(resCache)
    {
        var res = IoCManager.Resolve<IResourceCache>();
        var textureCloseButton = res.GetResource<TextureResource>("/Textures/cross.png").Texture;
        var gameFont = res.GetResource<FontResource>("/Fonts/Allerta-Regular.ttf");
        var gameFont10 = new VectorFont(gameFont, 10);
        var gameFont12 = new VectorFont(gameFont, 12);

        var scrollBarNormal = new StyleBoxFlat {
            BackgroundColor = Color.Gray.WithAlpha(0.35f), ContentMarginLeftOverride = 10,
            ContentMarginTopOverride = 10
        };

        var scrollBarHovered = new StyleBoxFlat {
            BackgroundColor = new Color(140, 140, 140).WithAlpha(0.35f), ContentMarginLeftOverride = 10,
            ContentMarginTopOverride = 10
        };

        var scrollBarGrabbed = new StyleBoxFlat {
            BackgroundColor = new Color(160, 160, 160).WithAlpha(0.35f), ContentMarginLeftOverride = 10,
            ContentMarginTopOverride = 10,
        };

        Stylesheet =  new Stylesheet(new StyleRule[] {
            Element<WindowRoot>()
                .Prop("background", Color.FromHex("#111111")),
            
            Element<SimulationControl>()
                .Prop("background", Color.Black),

            Element<PanelContainer>().Class("SimulationBackground")
                .Prop("panel", new StyleBoxFlat { BackgroundColor = Color.Black}),

            // Default font.
            Element()
                .Prop("font", gameFont12)
                .Prop("font-color", Color.Black),
            
            Element<Label>().Class("SimulationStatus")
                .Prop("font-color", Color.White),

            // VScrollBar grabber normal
            Element<VScrollBar>()
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarNormal),

            // VScrollBar grabber hovered
            Element<VScrollBar>().Pseudo(ScrollBar.StylePseudoClassHover)
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarHovered),

            // VScrollBar grabber grabbed
            Element<VScrollBar>().Pseudo(ScrollBar.StylePseudoClassGrabbed)
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarGrabbed),

            // HScrollBar grabber normal
            Element<HScrollBar>()
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarNormal),

            // HScrollBar grabber hovered
            Element<HScrollBar>().Pseudo(ScrollBar.StylePseudoClassHover)
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarHovered),

            // HScrollBar grabber grabbed
            Element<HScrollBar>().Pseudo(ScrollBar.StylePseudoClassGrabbed)
                .Prop(ScrollBar.StylePropertyGrabber, scrollBarGrabbed),

            // Window background default color.
            Element().Class(DefaultWindow.StyleClassWindowPanel)
                .Prop("panel", new StyleBoxFlat { BackgroundColor = Color.FromHex("#4A4A4A") }),

            // Window title properties
            Element().Class(DefaultWindow.StyleClassWindowTitle)
                // Color
                .Prop(Label.StylePropertyFontColor, Color.FromHex("#000000")),

            // Window header color.
            Element().Class(DefaultWindow.StyleClassWindowHeader)
                .Prop(PanelContainer.StylePropertyPanel, new StyleBoxFlat {
                    BackgroundColor = Color.FromHex("#636396"), Padding = new Thickness(1, 1)
                }),

            // Window close button
            Element().Class(DefaultWindow.StyleClassWindowCloseButton)
                // Button texture
                .Prop(TextureButton.StylePropertyTexture, textureCloseButton)
                // Normal button color
                .Prop(Control.StylePropertyModulateSelf, Color.FromHex("#000000")),

            // Window close button hover color
            Element().Class(DefaultWindow.StyleClassWindowCloseButton).Pseudo(TextureButton.StylePseudoClassHover)
                .Prop(Control.StylePropertyModulateSelf, Color.FromHex("#505050")),

            // Window close button pressed color
            Element().Class(DefaultWindow.StyleClassWindowCloseButton).Pseudo(TextureButton.StylePseudoClassPressed)
                .Prop(Control.StylePropertyModulateSelf, Color.FromHex("#808080")),

            // Button style normal
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Pseudo(ContainerButton.StylePseudoClassNormal)
                .Prop(ContainerButton.StylePropertyStyleBox, new StyleBoxFlat { BackgroundColor = Color.FromHex("#C0C0C0"), BorderThickness = new Thickness(1), BorderColor = Color.FromHex("#707070"), ContentMarginLeftOverride = 3, ContentMarginTopOverride = 3}),

            // Button style hovered
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Pseudo(ContainerButton.StylePseudoClassHover)
                .Prop(ContainerButton.StylePropertyStyleBox, new StyleBoxFlat { BackgroundColor = Color.FromHex("#D0D0D0"), BorderThickness = new Thickness(1), BorderColor = Color.FromHex("#707070"), ContentMarginLeftOverride = 3, ContentMarginTopOverride = 3}),

            // Button style pressed
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Pseudo(ContainerButton.StylePseudoClassPressed)
                .Prop(ContainerButton.StylePropertyStyleBox, new StyleBoxFlat { BackgroundColor = Color.FromHex("#E0E0E0"), BorderThickness = new Thickness(1), BorderColor = Color.FromHex("#707070"), ContentMarginLeftOverride = 3, ContentMarginTopOverride = 3}),

            // Button style disabled
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Pseudo(ContainerButton.StylePseudoClassDisabled)
                .Prop(ContainerButton.StylePropertyStyleBox, new StyleBoxFlat { BackgroundColor = Color.FromHex("#FAFAFA"), BorderThickness = new Thickness(1), BorderColor = Color.FromHex("#707070"), ContentMarginLeftOverride = 3, ContentMarginTopOverride = 3}),

            // CheckBox unchecked
            Element<TextureRect>().Class(CheckBox.StyleClassCheckBox)
                .Prop(TextureRect.StylePropertyTexture, Texture.Black), // TODO: Add actual texture instead of this.

            // CheckBox unchecked
            Element<TextureRect>().Class(CheckBox.StyleClassCheckBox, CheckBox.StyleClassCheckBoxChecked)
                .Prop(TextureRect.StylePropertyTexture, Texture.White), // TODO: Add actual texture instead of this.

            // LineEdit
            Element<LineEdit>()
                // background color
                .Prop(LineEdit.StylePropertyStyleBox, new StyleBoxFlat{ BackgroundColor = Color.FromHex("#D3B5B5"), BorderThickness = new Thickness(1), BorderColor = Color.FromHex("#abadb3")})
                // default font color
                .Prop("font-color", Color.Black)
                .Prop("cursor-color", Color.Black),

            // LineEdit non-editable text color
            Element<LineEdit>().Class(LineEdit.StyleClassLineEditNotEditable)
                .Prop("font-color", Color.FromHex("#363636")),

            // LineEdit placeholder text color
            Element<LineEdit>().Pseudo(LineEdit.StylePseudoClassPlaceholder)
                .Prop("font-color", Color.FromHex("#7d7d7d")),

            // TabContainer
            Element<TabContainer>()
                // Panel style
                .Prop(TabContainer.StylePropertyPanelStyleBox, new StyleBoxFlat { BackgroundColor = Color.White, BorderThickness = new Thickness(1), BorderColor = Color.Black})
                // Active tab style
                .Prop(TabContainer.StylePropertyTabStyleBox, new StyleBoxFlat {
                    BackgroundColor = Color.FromHex("#707070"), PaddingLeft = 1, PaddingRight = 1, ContentMarginLeftOverride = 5, ContentMarginRightOverride = 5
                })
                // Inactive tab style
                .Prop(TabContainer.StylePropertyTabStyleBoxInactive, new StyleBoxFlat {
                    BackgroundColor = Color.FromHex("#D0D0D0"), PaddingLeft = 1, PaddingRight = 1, ContentMarginLeftOverride = 5, ContentMarginRightOverride = 5
                })
                .Prop("font", gameFont10),

        });
    }

    public override Stylesheet Stylesheet { get; }
}