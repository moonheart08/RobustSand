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
    public abstract Stylesheet Stylesheet { get; }

    protected StylesheetBase(IResourceCache resCache) { }
}