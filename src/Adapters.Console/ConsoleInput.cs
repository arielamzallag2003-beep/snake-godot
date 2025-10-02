using Elysium.Foundation.Serpentis.Core.Domain;

internal static class ConsoleInput
{
    public static bool Poll(out InputAction? action, out bool quitRequested)
    {
        action = null;
        quitRequested = false;

        if (!Console.KeyAvailable)
            return false;

        var key = Console.ReadKey(intercept: true).Key;
        switch (key)
        {
            // --- Movement (arrows) ---
            case ConsoleKey.UpArrow:    action = InputAction.TurnUp; break;
            case ConsoleKey.DownArrow:  action = InputAction.TurnDown; break;
            case ConsoleKey.LeftArrow:  action = InputAction.TurnLeft; break;
            case ConsoleKey.RightArrow: action = InputAction.TurnRight; break;

            // --- Movement (ZQSD for AZERTY) ---
            case ConsoleKey.Z: action = InputAction.TurnUp; break;
            case ConsoleKey.S: action = InputAction.TurnDown; break;
            case ConsoleKey.Q: action = InputAction.TurnLeft; break;
            case ConsoleKey.D: action = InputAction.TurnRight; break;

            // --- Other controls ---
            case ConsoleKey.P: action = InputAction.PauseToggle; break;
            case ConsoleKey.R: action = InputAction.Restart; break;
            case ConsoleKey.Escape: quitRequested = true; break;
            default: return false;
        }

        return true;
    }
}
