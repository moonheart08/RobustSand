using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Timing;

namespace Content.Client.GameView;

public sealed class GameEditorState : State
{
    private IUserInterfaceManager _userInterface = default!;
    private readonly GameEditorView _view = new();
    
    public override void Startup()
    {
        _userInterface = IoCManager.Resolve<IUserInterfaceManager>();
        _userInterface.StateRoot.AddChild(_view);
    }

    public override void Shutdown()
    {
        _userInterface.StateRoot.RemoveChild(_view);
    }
    
    
    public override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
    }
}