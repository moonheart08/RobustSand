using System.Globalization;
using Content.Client.GameView;
using Content.Client.Input;
using Content.Client.Simulation;
using Content.Client.UI;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Timing;

// DEVNOTE: You can change the namespace and project name to whatever you want!
// Just make sure to consistently use a prefix across your different projects.
namespace Content.Client;

public sealed class EntryPoint : GameClient
{
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;

    public override void PreInit()
    {
        base.PreInit();

        IoCManager.InjectDependencies(this);

        IoCManager.Resolve<ILocalizationManager>()
            .LoadCulture(new CultureInfo(_cfgManager.GetCVar(GameConfigVars.GameLocale)));
    }

    public override void Init()
    {
        var factory = IoCManager.Resolve<IComponentFactory>();

        // DEVNOTE: Registers all of your components.
        factory.DoAutoRegistrations();

        IoCRegistrations.Register();

        IoCManager.BuildGraph();
            
        factory.GenerateNetIds();
        
        if (_cfgManager.GetCVar<float>(CVars.DisplayUIScale) < 1.0f)
            _cfgManager.SetCVar("display.uiScale", 1.0f);

        // DEVNOTE: This is generally where you'll be setting up the IoCManager further, like the tile manager.
    }

    public override void PostInit()
    {
        base.PostInit();

        // DEVNOTE: Further setup..
            
        // DEVNOTE: This starts the singleplayer mode,
        // which means you can start creating entities, spawning things...
        // If you want to have a main menu to start the game from instead, use the StateManager.
        _client.StartSinglePlayer();
        _client.PlayerNameOverride = "three";
        _stateManager.RequestStateChange<GameEditorState>();
        IoCManager.Resolve<StylesheetManager>().Initialize();
        var inputMan = IoCManager.Resolve<IInputManager>();
        ContentContexts.SetupContexts(inputMan.Contexts);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
            
        // DEVNOTE: You'll want to do a proper shutdown here.
    }

    public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
    {
        base.Update(level, frameEventArgs);
    }
}