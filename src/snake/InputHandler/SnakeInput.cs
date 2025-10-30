using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.InputHandler
{
    public static class SnakeInput
    {

        public static Direction Handle(InputEvent @event, SnakeGame engine, Direction currentDirection)
        {

            if (@event is InputEventKey eventKey && eventKey.Pressed)
            {
                switch (eventKey.Keycode)
                {
                    case Key.Z:
                        engine.HandleInput(InputAction.TurnUp);
                        return Direction.Up;
                    case Key.S:
                        engine.HandleInput(InputAction.TurnDown);
                        return Direction.Down;
                    case Key.Q:
                        engine.HandleInput(InputAction.TurnLeft);
                        return Direction.Left;
                    case Key.D:
                        engine.HandleInput(InputAction.TurnRight);
                        return Direction.Right;
                    case Key.R:
                        engine.HandleInput(InputAction.Restart);
                        break;
                }
            }

            return currentDirection;
        }
    }
}
