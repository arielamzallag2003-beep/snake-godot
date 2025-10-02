using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Engine;
using Elysium.Foundation.Serpentis.Core.Domain;

class Program
{
    static void Main()
    {
        const int W = 24, H = 16;
        const int InitialLen = 4;
        const int SafePad = 2;

        var cfg = new GameConfig(
            width: W, height: H, tickSeconds: 0.12,
            wrapEdges: false, initialLength: InitialLen,
            fragmentChance: 0.08, entropyThresholdTicks: 80,
            engravingLifespanRuns: 3,
            safeSpawnPadding: SafePad
        );

        var game = new SnakeGame();
        game.Initialize(cfg, seed: 1337);

        var time = new ConsoleTime();
        var renderer = new ConsoleRenderer(W, H); // changed: no InitialLen/SafePad needed

        bool running = true;
        while (running)
        {
            if (ConsoleInput.Poll(out InputAction? action, out bool quit))
            {
                if (quit) break;
                if (action is InputAction a) game.HandleInput(a);
            }

            var dt = time.NextDeltaSeconds();
            if (game.Update(dt))
            {
                var snapshot = game.GetSnapshot();
                renderer.Draw(snapshot);
            }

            Thread.Sleep(1);
        }

        Console.CursorVisible = true;
    }
}
