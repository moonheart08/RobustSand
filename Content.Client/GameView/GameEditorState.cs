using System;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;
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
        _userInterface.WindowRoot.Window!.Resized += args =>
        {
            UpdateScale(args.NewSize);
        };
        UpdateScale(_userInterface.WindowRoot.Window!.Size);
        IoCManager.Resolve<IInputManager>().Contexts.SetActiveContext("editor");
    }

    private void UpdateScale(Vector2i newSize)
    {
        var configurationManager = IoCManager.Resolve<IConfigurationManager>();
        var scale = ((Vector2) newSize / 768.0f);
        configurationManager.SetCVar("display.uiScale", Math.Max(Math.Min(scale.X, scale.Y), 1.0f));
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