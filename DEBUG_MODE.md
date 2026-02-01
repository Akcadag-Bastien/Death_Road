# Player Debug Mode

This project now exposes the Rendering Debugger at runtime through the `GameManager`
component (`Assets/Scripts/GameManager.cs`).

- Press `F1` (configurable through `toggleDebugKey`) to instantly show or hide the
  debug UI. The standard combination `Left Ctrl + Backspace` still works because the
  Rendering Debugger input axes remain in `ProjectSettings/InputManager.asset`.
- Enable `Show Debug Menu On Start` in the inspector if you want the UI to be
  visible as soon as the player scene loads.
- Keep `Allow Debug In Release Builds` enabled when you need these tools outside of
  development builds. This forces the `DebugUpdater` helper to exist even in release
  players so the Rendering Debugger can be opened.
- Assign the player's `PlayerMovement` component to `GameManager.playerMovement`
  (the script tries to auto-fill it). While the debug UI is visible the player
  enters noclip: collisions are disabled, WASD becomes free-fly movement, `Space`
  ascends, and `Left Shift` descends. Adjust the flight speeds and bindings inside
  `PlayerMovement` under the "Debug Fly/Noclip" header.

Attach the `GameManager` script to any always-present object (a `GameManager`
object already exists in `SampleScene`) to make the debug toggle available to the
player.
