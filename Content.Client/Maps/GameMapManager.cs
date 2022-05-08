using System;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Maps;

/// <summary>
/// Pain, suffering even. This manages the game's levels both in-game and in-editor.
/// </summary>
[PublicAPI]
public sealed class GameMapManager
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;
    
    public void Initialize()
    {
        _lightManager.AmbientLightColor = Color.Magenta.WithAlpha(0.2f);
    }

    public bool LoadGameLevel(GameMapPrototype mapPrototype)
    {
        throw new NotImplementedException();
    }
}