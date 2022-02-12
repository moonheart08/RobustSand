using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Editor;

public sealed class GameEditorState : State
{
    private IUserInterfaceManager _userInterface = default!;
    private IOverlayManager _overlayManager = default!;
    private IEyeManager _eyeManager = default!;
    private readonly GameEditorView _view = new GameEditorView();
    
    public override void Startup()
    {
        _userInterface = IoCManager.Resolve<IUserInterfaceManager>();
        _eyeManager = IoCManager.Resolve<IEyeManager>();
        _overlayManager = IoCManager.Resolve<IOverlayManager>();
        _overlayManager.AddOverlay(new ViewBackgroundOverlay());

        _userInterface.StateRoot.AddChild(_view);
    }

    public override void Shutdown()
    {
        _overlayManager.RemoveOverlay(typeof(ViewBackgroundOverlay));
        _userInterface.StateRoot.RemoveChild(_view);
    }
    
    public override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        //_view.Viewport.Eye = _eyeManager.CurrentEye;
    }
}