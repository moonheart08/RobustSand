using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Content.Client.UI;

public sealed class StylesheetManager
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    
    public Stylesheet EditorSheet { get; private set; } = default!;

    public void Initialize()
    {
        EditorSheet = new EditorStylesheet(_resourceCache).Stylesheet;
        _userInterfaceManager.Stylesheet = EditorSheet;
    }
}