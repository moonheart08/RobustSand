using System.Linq;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Content.Client.UI;

public sealed class EditorStylesheet : StylesheetBase
{
    public EditorStylesheet(IResourceCache resCache) : base(resCache)
    {
        Stylesheet = new Stylesheet(BaseRules.Concat(System.Array.Empty<StyleRule>()).ToList());
    }

    public override Stylesheet Stylesheet { get; }
}